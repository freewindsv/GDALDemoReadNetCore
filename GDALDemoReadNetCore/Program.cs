using OSGeo.OGR;
using System;

namespace GDALDemoRead
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: GDALDemoReadNetCore <datasource> <layername>");
                PauseAndExit(0);
            }

            Ogr.RegisterAll();

            int errCode = 0;
            using (DataSource ds = Ogr.Open(args[0], 0))
            {
                if (ds == null)
                {
                    Console.WriteLine("Open failed");
                    errCode = 1;
                }

                if (errCode == 0)
                {

                    Layer layer = ds.GetLayerByName(args[1]);
                    if (layer == null)
                    {
                        Console.WriteLine("Layer not found");
                        errCode = 2;
                    }

                    if (errCode == 0)
                    {
                        int n = 1;
                        Feature feature;
                        while ((feature = layer.GetNextFeature()) != null)
                        {
                            Console.WriteLine($"Feature #{n}");
                            PrintAttributes(feature);
                            PrintGeometry(feature);
                            n++;
                        }
                    }
                }
            }

            PauseAndExit(0);
        }

        static void PauseAndExit(int errCode)
        {
            Console.ReadLine();
            Environment.Exit(errCode);
        }

        static void PrintAttributes(Feature feature)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("  Attributes:");
            Console.ForegroundColor = oldColor;
            int fieldsCount = feature.GetFieldCount();
            for (int i = 0; i < fieldsCount; i++)
            {
                object val = null;
                string fName = feature.GetFieldDefnRef(i).GetName();
                FieldType fType = feature.GetFieldType(i);
                switch (fType)
                {
                    case FieldType.OFTString:
                        val = feature.GetFieldAsString(i);
                        break;
                    case FieldType.OFTInteger:
                        val = feature.GetFieldAsInteger(i);
                        break;
                    case FieldType.OFTInteger64:
                        val = feature.GetFieldAsInteger64(i);
                        break;
                    case FieldType.OFTReal:
                        val = feature.GetFieldAsDouble(i);
                        break;
                    default:
                        val = "<unknown>";
                        break;
                }
                Console.WriteLine($"  Name:{fName}, Value:{val}");
            }
        }

        static void PrintGeometry(Feature feature)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  Geometry:");
            Console.ForegroundColor = oldColor;
            PrintGeometry(feature.GetGeometryRef(), "  ");
        }

        static void PrintGeometry(Geometry geom, string padding)
        {
            Console.WriteLine($"{padding}GeometryName:{geom.GetGeometryName()}, GeometryType:{geom.GetGeometryType()}");
            int pointCount = geom.GetPointCount();
            for (int j = 0; j < pointCount; j++)
            {
                double[] points = new double[3];
                geom.GetPoint(j, points);
                Console.WriteLine($"{padding}x:{points[0]}, y:{points[1]}");
            }

            int geomCount = geom.GetGeometryCount();
            for (int i = 0; i < geomCount; i++)
            {
                PrintGeometry(geom.GetGeometryRef(i), padding + "  ");
            }
        }
    }
}