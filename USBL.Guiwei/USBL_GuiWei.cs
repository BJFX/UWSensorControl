using System;
using System.Collections.Generic;
using System.Text;


namespace USBL.GuiWei
{
    public class USBL_GuiWei
    {
       /* private float x = (float)-903.5114, y = (float)652.0620, z = (float)-870.917;
        private float heading = 100, pitch = 5, roll = -5;
        private float LonLocal = 0, LatLocal = 0;
        private float usx = 0, usy = 0, usz = 0;
        private float TravelTime = (float)0.9428125;*/

        private double _X_carrier, _Y_carrier,_Z_carrier;
        private double _phi, _gama;
        public double x_local, y_local, z_local;
        public double LonTarget, LatTarget, DepthTarget;

        public USBL_GuiWei(float x_carrier,float y_carrier,float z_carrier,float Heave,float Heading,
            float Pitch, float Roll, float Lon_local, float Lat_local, float us_x, float us_y, float us_z, float Travel_time, float ArrayDepth, int[] SVPd, double[] SVPc)
        {
                trans_coor(x_carrier,y_carrier,z_carrier,Heading,Pitch,Roll);
                direction(_X_carrier, _Y_carrier, _Z_carrier);
                CorrectionConstC PositionSoundMend = new CorrectionConstC(Travel_time, _phi, _gama, ArrayDepth, Heave, SVPd, SVPc);
                PositionSoundMend.SoundSpeedMend();
                Translation(PositionSoundMend.ResultX, PositionSoundMend.ResultY, -PositionSoundMend.ResultH + ArrayDepth, us_x, us_y, us_z);

            
            LongLagtitude(Lon_local, Lat_local);


        }

        public void trans_coor(float x, float y, float z, float heading, float pitch, float roll)
        {
            double x1, y1, z1, x2, y2, z2;
            x1 = x;
            y1 = Math.Cos(roll * Math.PI / 180) * y - Math.Sin(roll * Math.PI / 180) * z;
            z1 = Math.Sin(roll * Math.PI / 180) * y + Math.Cos(roll * Math.PI / 180) * z;

            x2 = Math.Cos(pitch * Math.PI / 180) * x1 - Math.Sin(pitch * Math.PI / 180) * z1;
            y2 = y1;
            z2 = Math.Sin(pitch * Math.PI / 180) * x1 + Math.Cos(pitch * Math.PI / 180) * z1;

            _X_carrier = Math.Cos(heading * Math.PI / 180) * x2 + Math.Sin(heading * Math.PI / 180) * y2;
            _Y_carrier = -Math.Sin(heading * Math.PI / 180) * x2 + Math.Cos(heading * Math.PI / 180) * y2;
            _Z_carrier = z2;
        }

        public void direction(double x, double y, double z)
        {
            if (x > 0)
                if (y >= 0)
                    _phi = Math.Atan(y / x);
                else
                    _phi = Math.Atan(y / x) + 2 * Math.PI;
            else
                if (x < 0)
                    _phi = Math.Atan(y / x) + Math.PI;
                else
                    if (x == 0 & y > 0)
                        _phi = Math.PI / 2;
                    else
                        _phi = 3 * Math.PI / 2;
            _gama = Math.Atan(Math.Abs(y / (-z * Math.Sin(_phi))));

        }

        public void Translation(double x, double y, double z, double usx, double usy, double usz)
        {
            x_local = x + usx;
            y_local = y + usy;
            z_local = z + usz;
        }

        public void LongLagtitude(double LonLocal, double LatLocal)
        {
            LonTarget = LonLocal + x_local / (111.3 * 1000 * Math.Abs(Math.Cos(LatLocal)));
            LatTarget = LatLocal + y_local / (111.3 * 1000);
            DepthTarget = z_local;
        }
    }
}
