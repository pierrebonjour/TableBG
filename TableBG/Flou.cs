using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TableBG
{
    class Flou
    {
        public Double y = 0;
        private Double minInterval = 0;
        private Double maxInterval = 0;
        private Double minIntervalValue = 0;
        private Double maxIntervalValue = 0;
        private int nbPointsInArray = 0;
        private Double[,] points = null;

        public Flou(Double[,] points)
        {
            //remplir min, max et points, et tout ce qui peut aider...
            this.points = points;
            this.nbPointsInArray = points.Length/2;       
            this.minInterval = points[0,0];
            this.minIntervalValue = points[0, 1];
            this.maxInterval = points[this.nbPointsInArray - 1,0];
            this.maxIntervalValue = points[this.nbPointsInArray - 1, 1];
        }

        public Flou(Double y)
        {
            this.y = y;
        }

        public Flou(Flou f)
        {
            this.y = f.y;
        }

        public Flou x(Double val)
        {
            if (val <= minInterval) return new Flou(minIntervalValue);
            if (val >= maxInterval) return new Flou(maxIntervalValue);

            for (int i=1; i<this.nbPointsInArray; i++)
            {
                if (val<this.points[i, 0])
                {
                    return new Flou(Flou.extrapolate(this.points[i - 1, 0], this.points[i - 1, 1], this.points[i, 0], this.points[i, 1], val));
                }
            }
            throw new Exception("Internal error");
        }

        private static Double extrapolate(Double x1, Double y1, Double x2, Double y2, Double xVal)
        {
            Double A = (y2 - y1) / (x2 - x1);
            Double B = y1 - A * x1;
            return A * xVal + B;
        }

        public static Flou operator &(Flou f1, Flou f2)
        {
            return new Flou(Math.Min(f1.y, f2.y));
        }

        public static Flou operator |(Flou f1, Flou f2)
        {
            return new Flou(Math.Max(f1.y, f2.y));
        }


    }


}
