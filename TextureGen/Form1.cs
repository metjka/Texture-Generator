using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace TextureGen {
    public partial class Form1 : Form {
        public System.Random r;

        public Form1() {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e) {


            //8767
            Bitmap tmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            float[,] Values = new float[pictureBox1.Width, pictureBox1.Height];//сетка пикселей

            int wid = pictureBox1.Width;//высота пиктчюбокса
            int hei = pictureBox1.Height;//длинна пикчербокса

            int NPoints = int.Parse(pointNumTxt.Text);//количество точек

            bool useValueArray = false;//проверка программы

            r = getrRandom();//получение рандомного значения 

            int colorR = int.Parse(textBox1.Text);//получение цвета
            int colorG = int.Parse(textBox2.Text);//получение цвета
            int colorB = int.Parse(textBox3.Text);//получение цвета

            for (int nr = 0; nr < NPoints; nr++) {//выбирание точек -- 50 и генерирование каждой

                int x = r.Next(0, wid - 1);//получение координаты точки
                int y = r.Next(0, hei - 1);//получение координаты точки

                int fromP = int.Parse(PowerFromTxt.Text);//сила свичения от
                int toP = int.Parse(PowerToTxt.Text);//сила свичения до
                int fromR = int.Parse(CrSizeFrom.Text);//размер точки от 
                int toR = int.Parse(CrSizeTo.Text);//размер точки до
                //использыавние функций
                switch (cratesFunctionCbo.Text.ToUpper()) {
                    case "STARFIELD":
                        Starfield(Values, wid, hei, x, y, fromP, toP, fromR, toR);
                        useValueArray = true;
                        break;
                    case "LANDSCAPE1":
                        landscape1(Values, wid, hei, x, y, fromP, toP, fromR, toR);
                        useValueArray = true;
                        break;
                    case "LINE":
                        line(Values, wid, hei, NPoints);
                        useValueArray = true;
                        break;
                    default:
                        break;
                }
            }


            if (useValueArray) {
                if (scaledChk.Checked)
                    updBitmapScaled(Values, tmp, grayScale.Checked);
                else
                    updBitmap(Values, tmp, grayScale.Checked, float.Parse(minLimitTxt.Text), float.Parse(maxLimitTxt.Text), colorR, colorG, colorB);
            }

            pictureBox1.Image = tmp;
            pictureBox1.Refresh();
        }

        private void updLine(float[,] Values, Bitmap tmp) {
            throw new NotImplementedException();
        }

        private Random getrRandom() {
            if (randomChk.Checked) {
                int seed = (int)DateTime.Now.Ticks;
                seedTxt.Text = seed.ToString();
                return new Random(seed);
            }
            else {
                int seed;
                if (int.TryParse(seedTxt.Text, out seed)) {
                    return new Random(seed);
                }
                else {
                    seed = (int)DateTime.Now.Ticks;
                    seedTxt.Text = seed.ToString();
                    return new Random(seed);
                }

            }
        }

        private void Starfield(float[,] Values, int wid, int hei, int x, int y, int fromP, int toP, int fromR, int toR) {
            double power = r.Next(fromP, toP);  //сила точки
            int radius = r.Next(fromR, toR);    //радиус точки
            for (int i = 0; i < wid; i++) {     //от ширины 
                for (int j = 0; j < hei; j++) { //до высоты

                    Double Dist1X = Math.Abs((i - x));
                    Double Dist1Y = Math.Abs((j - y));

                    Double Dist2X = wid - Dist1X;
                    Double Dist2Y = hei - Dist1Y;

                    Dist1X = Math.Min(Dist1X, Dist2X);
                    Dist1Y = Math.Min(Dist1Y, Dist2Y);

                    double Dist = Math.Sqrt(Math.Pow(Dist1X, 2) + Math.Pow(Dist1Y, 2));
                    if (Dist < 0.001) Dist = 0.001;          //avoid division by zero
                        

                    int pixX = (i);
                    int pixY = (j);

                    Values[pixX, pixY] = Values[pixX, pixY] + (int)(power * radius / Dist);

                }

            }

        }

        private void landscape1(float[,] Values, int wid, int hei, int x, int y, int fromP, int toP, int fromR, int toR) {
            double power = r.Next(fromP, toP);
            int radius = r.Next(fromR, toR);
            for (int i = 0; i < wid; i++) {
                for (int j = 0; j < hei; j++) {

                    Double Dist1X = Math.Abs((i - x));
                    Double Dist1Y = Math.Abs((j - y));

                    Double Dist2X = wid - Dist1X;
                    Double Dist2Y = hei - Dist1Y;
                    /*to grant seamless I take the min between distX and wid-distX
                     |                       |
                     |                       |     ----------- = Dist1X
                     |...i-----------X.......|     ..........  = Dist2X
                     |                       |
                     */
                    Dist1X = Math.Min(Dist1X, Dist2X);
                    /*to grant seamless I take the min between distY and hei-distY*/
                    Dist1Y = Math.Min(Dist1Y, Dist2Y);

                    double Dist = Math.Sqrt(Math.Pow(Dist1X, 2) + Math.Pow(Dist1Y, 2));
                    if (Dist < 0.001) //avoid division by zero
                        Dist = 0.001;

                    int pixX = (i);
                    int pixY = (j);


                    Values[pixX, pixY] = Values[pixX, pixY] + (int)(power * radius / Math.Log(Dist + 1));


                }

            }

        }

        private void line(float[,] Values, int wid, int hei, int N){
            int siseOfLine = hei / N;
            int BNW = 0;
            int a = 0;
            for (int i = 0; i < wid; i++) {
                if (BNW == (siseOfLine)) {
                    a = setTF(a);
                    BNW = 0;
                }
                BNW++;
                for (int j = 0; j < hei; j++) {
                    if (a == 0) {
                        Values[j,i] = 0; //black
                    }else if (a == 1) {
                        Values[j,i] = 255; //white

                    }
                }
            }
        }
        private int setTF (int a) {
            if (a == 0) {
                a = 1;
                return a;
            }
            else {
                a = 0;
                return a;
            }
        }
        private void updBitmapScaled(float[,] Values, Bitmap tmp, bool greyscale) {
            float max = 0f;
            float min = 255f;
            for (int i = 0; i < Values.GetLength(0); i++) {
                for (int j = 0; j < Values.GetLength(1); j++) {
                    if (Values[i, j] > max)
                        max = Values[i, j];
                    if (Values[i, j] < min)
                        min = Values[i, j];
                }
            }

            float d = max - min;

            for (int i = 0; i < Values.GetLength(0); i++) {
                for (int j = 0; j < Values.GetLength(1); j++) {
                    float v = (Values[i, j] / d) * 255f;

                    if (v < 0)
                        v = 0;

                    if (v > 255f)
                        v = 255f;

                    Color clr;
                    if (greyscale)
                        clr = Color.FromArgb((int)v, (int)v, (int)v);
                    else
                        clr = Color.FromArgb((int)v, 255, 255, 255);
                    tmp.SetPixel(i, j, clr);

                }
            }

        }

        private void updBitmap(float[,] Values, Bitmap tmp, bool greyscale, float minLimit, float maxLimit, int R, int G, int B) {
            for (int i = 0; i < Values.GetLength(0); i++) {
                for (int j = 0; j < Values.GetLength(1); j++) {
                    float v = Values[i, j];

                    if (v < minLimit) v = 0;

                    if (v > maxLimit) v = 255f;

                    Color clr;
                    if (greyscale)
                        clr = Color.FromArgb((int)Math.Abs(v - R), (int)Math.Abs(v - G), (int)Math.Abs(v - B));
                        //clr = Color.FromArgb((int)r.Next(R), (int)r.Next(G), (int)r.Next(B));

                    else
                        clr = Color.FromArgb((int)v, R, G, B);
                    tmp.SetPixel(i, j, clr);

                }
            }

        }

        private void button2_Click(object sender, EventArgs e) {
            Bitmap bmpSave = (Bitmap)pictureBox1.Image;
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "png";
            sfd.Filter = "Image files (*.png)|*.png|All files (*.*)|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)
                bmpSave.Save(sfd.FileName, ImageFormat.Png);
        }

        private void Form1_Load(object sender, EventArgs e) {

        }


    }
}
