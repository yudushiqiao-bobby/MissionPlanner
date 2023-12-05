using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using KMLib;
using netDxf;
using netDxf.Entities;
using netDxf.Tables;
using static MissionPlanner.Utilities.Pelco;

namespace MissionPlanner.Utilities
{
    public class dxf
    {
        public delegate void LineEventHandler(dxf sender, Line line);
        public delegate void PolyLineEventHandler(dxf sender, Polyline pline);
        public delegate void LwPolylineEventHandler(dxf sender, LwPolyline pline);
        public delegate void MLineEventHandler(dxf sender, MLine pline);

        public event LineEventHandler newLine;
        public event PolyLineEventHandler newPolyLine;
        public event LwPolylineEventHandler newLwPolyline;
        public event MLineEventHandler newMLine;

        public object Tag;
        private DxfDocument document=null ;

        public void Read(string filename)
        {
            var dxfDocument = DxfDocument.Load(filename);

            foreach (var line in dxfDocument.Lines)
            {
                if(line.IsVisible)
                    newLine?.Invoke(this, line);
            }

            foreach (var pline in dxfDocument.Polylines)
            {
                if (pline.IsVisible)
                    newPolyLine?.Invoke(this, pline);
            }

            foreach (var lwpline in dxfDocument.LwPolylines)
            {
                if (lwpline.IsVisible)
                    newLwPolyline?.Invoke(this, lwpline);
            }

            foreach (var mline in dxfDocument.MLines)
            {
                if (mline.IsVisible)
                    newMLine?.Invoke(this, mline);
            }
        }


        public bool Create()
        {
            try
            {
                document = new DxfDocument(netDxf.Header.DxfVersion.AutoCad2007);
                return true;
            }
            catch (Exception ee)
            {

                return false;
            }
          
        }

        public bool AddPolygon(PointF[] points)
        {
            try
            {
                if (document == null) return false;

                Layer layer = new Layer("ydsqlayer");//新建图层
                layer.Color = AciColor.Yellow;//设置图层默认颜色


                LwPolyline poly = new LwPolyline();
                poly.Layer = layer;
                foreach (var p in points)
                {
                    poly.Vertexes.Add(new LwPolylineVertex(new Vector2(p.X, p.Y)));
                }
                poly.Vertexes.Add(new LwPolylineVertex(new Vector2(points[0].X, points[0].Y)));
                poly.Vertexes[2].Bulge = 1;
                poly.IsClosed = true;//封闭图形
                document.AddEntity(poly);
                return true;
            }
            catch (Exception ee)
            {
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="isHex">  保存文件,第二个参数true是保存为二进制格式，false是保存为文本格式</param>
        /// <returns></returns>
        public bool Save(string filename,bool isHex)
        {
            try
            {
                if (document == null) return false;

                document.Save(filename,isHex);

                return true;
            }
            catch (Exception ee)
            {
                return false;
            }

        }



    }
}
