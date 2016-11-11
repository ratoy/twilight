using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;

namespace twilight
{
	class ShpReader
	{
		string m_ShpFolder = "", m_ShpName = "";
		EnumGeoType m_GeoType = EnumGeoType.Unknown;
		int m_FeatureCount = 0;
		double m_XMax = double.MaxValue, m_XMin = double.MinValue, m_YMax = double.MaxValue, m_Ymin = double.MinValue;
		FileStream m_fsShapeIndex = null;
		BinaryReader m_brShapeIndex = null;
		FileStream m_fsShapeFile = null;
		BinaryReader m_brShapeFile = null;

		public ShpReader (string FullShpFileName)
		{
			string ShpFolder = System.IO.Path.GetDirectoryName (FullShpFileName);
			string ShpName = System.IO.Path.GetFileName (FullShpFileName);
			
			Init (ShpFolder, ShpName);
		}

		public ShpReader (string ShpFolder, string ShpName)
		{
			Init (ShpFolder, ShpName);
		}

		void Init (string ShpFolder, string ShpName)
		{
			m_ShpFolder = ShpFolder;
			if (ShpName.EndsWith (".shp")) {
				m_ShpName = System.IO.Path.GetFileNameWithoutExtension (ShpName);
			} else {
				m_ShpName = ShpName;
			}
			//check shapefiles
			string[] ShpFiles = new string[] { ".shp", ".dbf", ".shx" };
			foreach (string str in ShpFiles) {
				if (!System.IO.File.Exists (System.IO.Path.Combine (m_ShpFolder, m_ShpName + str))) {
					System.Diagnostics.Debug.Print ("invaild shape file!");
					return;
				}
			}

			//shp
			string ShpFile = System.IO.Path.Combine (m_ShpFolder, m_ShpName + ".shp");
			m_fsShapeFile = new FileStream (ShpFile, FileMode.Open, FileAccess.Read);
			m_brShapeFile = new BinaryReader (m_fsShapeFile);

			//shx
			ParseHeader (); 

			//dbf
			string DbfFile = System.IO.Path.Combine (m_ShpFolder, m_ShpName + ".dbf");
		}

		public int FeatureCount {
			get { return m_FeatureCount; }
		}

		public EnumGeoType GeoType {
			get { return m_GeoType; }
		}

		public double Ymin {
			get { return m_Ymin; }
		}

		public double YMax {
			get { return m_YMax; }
		}

		public double XMin {
			get { return m_XMin; }
		}

		public double XMax {
			get { return m_XMax; }
		}

		private void ParseHeader ()
		{
			string ShxFile = System.IO.Path.Combine (m_ShpFolder, m_ShpName + ".shx");
			m_fsShapeIndex = new FileStream (ShxFile, FileMode.Open, FileAccess.Read);
			m_brShapeIndex = new BinaryReader (m_fsShapeIndex, System.Text.Encoding.Unicode);

			m_brShapeIndex.BaseStream.Seek (0, 0);
			//Check file header
			if (m_brShapeIndex.ReadInt32 () != 170328064) //File Code is actually 9994, but in Little Endian Byte Order this is '170328064'
				throw (new ApplicationException ("Invalid Shapefile Index (.shx)"));

			m_brShapeIndex.BaseStream.Seek (24, 0); //seek to File Length
			int IndexFileSize = SwapByteOrder (m_brShapeIndex.ReadInt32 ()); //Read filelength as big-endian. The length is based on 16bit words
			m_FeatureCount = (2 * IndexFileSize - 100) / 8; //Calculate FeatureCount. Each feature takes up 8 bytes. The header is 100 bytes

			m_brShapeIndex.BaseStream.Seek (32, 0); //seek to ShapeType
			m_GeoType = GetGeoType (m_brShapeIndex.ReadInt32 ());

			//Read the spatial bounding box of the contents
			m_brShapeIndex.BaseStream.Seek (36, 0); //seek to box
			m_XMin = m_brShapeIndex.ReadDouble ();
			m_Ymin = m_brShapeIndex.ReadDouble ();
			m_XMax = m_brShapeIndex.ReadDouble ();
			m_YMax = m_brShapeIndex.ReadDouble ();
		}

		public void Close ()
		{
			try {
				m_brShapeFile.Close ();
				m_brShapeIndex.Close ();
				m_fsShapeIndex.Close ();
				m_fsShapeFile.Close ();
			} catch {
			}
		}

		///<summary>
		///Swaps the byte order of an int32
		///</summary>
		/// <param name="i">Integer to swap</param>
		/// <returns>Byte Order swapped int32</returns>
		private int SwapByteOrder (int i)
		{
			byte[] buffer = BitConverter.GetBytes (i);
			Array.Reverse (buffer, 0, buffer.Length);
			return BitConverter.ToInt32 (buffer, 0);
		}

		EnumGeoType GetGeoType (int i)
		{
			EnumGeoType GeoType = EnumGeoType.Unknown;
			switch (i) {
			case 1:
			case 11:
			case 21:
				GeoType = EnumGeoType.Point;
				break;
			case 3:
			case 13:
			case 23:
				GeoType = EnumGeoType.Polyline;
				break;
			case 8:
			case 18:
			case 28:
				GeoType = EnumGeoType.MultiPoint;
				break;
			case 15:
			case 25:
			case 5:
				GeoType = EnumGeoType.Polygon;
				break;
			default:
				break;
			}
			return GeoType;
		}

		/// <summary>
		/// Gets the file position of the n'th shape
		/// </summary>
		/// <param name="n">Shape ID</param>
		/// <returns></returns>
		private int GetShapeIndex (uint n)
		{
			m_brShapeIndex.BaseStream.Seek (100 + n * 8, 0);  //seek to the position of the index
			return 2 * SwapByteOrder (m_brShapeIndex.ReadInt32 ()); //Read shape data position
		}

		/// <summary>
		/// Reads and parses the geometry with ID 'oid' from the ShapeFile
		/// </summary>
		/// <remarks><see cref="FilterDelegate">Filtering</see> is not applied to this method</remarks>
		/// <param name="oid">Object ID</param>
		/// <returns>geometry</returns>
		public IGeometry ReadGeometry (uint oid)
		{
			m_brShapeFile.BaseStream.Seek (GetShapeIndex (oid) + 8, 0); //Skip record number and content length
			EnumGeoType type = GetGeoType (m_brShapeFile.ReadInt32 ()); //Shape type
			if (type == EnumGeoType.Unknown) {
				return null;
			}
			IGeometry pGeometry = null;
			if (m_GeoType == EnumGeoType.Point) {
				return new Point (m_brShapeFile.ReadDouble (), m_brShapeFile.ReadDouble ());
			} else if (m_GeoType == EnumGeoType.MultiPoint) {
				m_brShapeFile.BaseStream.Seek (32 + m_brShapeFile.BaseStream.Position, 0); //skip min/max box
				int nPoints = m_brShapeFile.ReadInt32 (); // get the number of points
				if (nPoints == 0)
					return null;

				List<Point> PointList = new List<Point> ();
				for (int i = 0; i < nPoints; i++) {
					PointList.Add (new Point (m_brShapeFile.ReadDouble (), m_brShapeFile.ReadDouble ()));
				}

				pGeometry = new MultiPoint ();
				(pGeometry as MultiPoint).PointList = PointList;
				return pGeometry;
			} else if (m_GeoType == EnumGeoType.Polyline || m_GeoType == EnumGeoType.Polygon) {
				m_brShapeFile.BaseStream.Seek (32 + m_brShapeFile.BaseStream.Position, 0); //skip min/max box

				int nParts = m_brShapeFile.ReadInt32 (); // get number of parts (segments)
				if (nParts == 0)
					return null;
				int nPoints = m_brShapeFile.ReadInt32 (); // get number of points

				int[] segments = new int[nParts + 1];
				//Read in the segment indexes
				for (int b = 0; b < nParts; b++)
					segments [b] = m_brShapeFile.ReadInt32 ();
				//add end point
				segments [nParts] = nPoints;

				List<List<Point>> LineList = new List<List<Point>> ();
				for (int LineID = 0; LineID < nParts; LineID++) {
					List<Point> Line = new List<Point> ();
					for (int i = segments[LineID]; i < segments[LineID + 1]; i++) {
						Line.Add (new Point (m_brShapeFile.ReadDouble (), m_brShapeFile.ReadDouble ()));
					}
					//inner ring is clockwise
					if (m_GeoType == EnumGeoType.Polygon) {
						Line.Reverse ();
					}

					LineList.Add (Line);
				}

				if (m_GeoType == EnumGeoType.Polygon) {
					//polygon
					pGeometry = new Polygon ();
					List<Ring> RingList = new List<Ring> ();
					foreach (var item in LineList) {
						RingList.Add (new Ring (item));
					}
					(pGeometry as Polygon).RingList = RingList;
				} else {
					//polyline
					pGeometry = new Polyline ();
					List<Segment> SegList = new List<Segment> ();
					foreach (var item in LineList) {
						SegList.Add (new Segment (item));
					}
					(pGeometry as Polyline).SegmentList = SegList;
				}
				return pGeometry;
			} else {
				return null;
			}
		}
	}
}
