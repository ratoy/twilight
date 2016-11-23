using System;
using System.Collections.Generic;
using System.Drawing;

namespace twilight
{
	public class DrawShapeFile:DrawGeometry
	{
		
		public DrawShapeFile (int Width, int Height):base(Width,Height)
		{
			
		}

		public override void Draw(Graphics g, object obj, IStyle style = null)
		{
			string ShpFileName = Convert.ToString(obj);
			//read
			List<IGeometry> GeoList = ReadShpFile(ShpFileName);
			if (GeoList.Count == 0)
			{
				return;
			}
			//transform
			EnumGeoType GeoType = GeoList[0].GeoType;
			List<IGeometry> GeoListFs = (List<IGeometry>)TransGeometry(GeoList);
		
			//draw
			switch (GeoType)
			{
				case EnumGeoType.Point:
					DrawPoint(g, GeoListFs,style as PointStyle );
					break;
				case EnumGeoType.Polyline:
					DrawPolyline(g, GeoListFs, style as LineStyle);
					break;
				case EnumGeoType.Polygon:
					DrawPolygon(g,GeoListFs, style as FillStyle );
					break;
				default:
					break;
			} 
		}

		EnumGeoType GetGeoType(string ShpFile)
		{
			if (!System.IO.File.Exists(ShpFile))
			{
				return EnumGeoType.Unknown;
			}
			ShpReader ShpRd = new ShpReader(ShpFile);
			return ShpRd.GeoType;
		}

		List<IGeometry> ReadShpFile (string ShpFile)
		{
			List<IGeometry> GeoList = new List<IGeometry> ();
			ShpReader ShpRd = new ShpReader (ShpFile);
			
			int count = ShpRd.FeatureCount;
			for (uint i = 0; i < count; i++) {
				//geometry
				IGeometry g = ShpRd.ReadGeometry (i);
				GeoList.Add (g);
			}
			return GeoList;
		}
	}
}

