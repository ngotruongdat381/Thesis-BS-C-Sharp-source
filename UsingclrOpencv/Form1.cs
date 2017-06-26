using MYopencv;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Drawing.Drawing2D;
using System.IO;


namespace UsingclrOpencv
{
    public partial class Form1 : Form
    {
        MYGui cvGui;
        private System.Drawing.Drawing2D.GraphicsPath mousePath;
        string source_path;
        Image mainImage = null;
        double resizeFactor;
        double paddingVertical;
        double paddingHorizontal;
        private List<PointF> userInput;
        string outputText;  //for testing only
        string currentFileName;

        public Form1()
        {
            InitializeComponent();
            cvGui = new MYGui();
            textBox1.Text = "D:\\tattoo.jpg";
            mousePath = new System.Drawing.Drawing2D.GraphicsPath();
            userInput = new List<PointF>();

        }
       

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap result = cvGui.ImageProcessing(source_path, userInput);
            //pictureBox1.Image = new Bitmap("yourImage.jpg"); 

            pictureBox2.Image = result;//set processed image to picture box

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Save new input
            string noFile = currentFileName.Substring(Math.Max(0, currentFileName.Length - 6));
            noFile = noFile.Substring(0, 2);
            StreamWriter file = new System.IO.StreamWriter("D:\\605\\Source code\\UserInput\\input_" + noFile + ".txt");
            file.WriteLine(outputText);
            file.Close();

            //Show new input lines
            Bitmap clone_img = new Bitmap(pictureBox1.Image);
            if (pictureBox2.Image != null)
            {
                clone_img = new Bitmap(pictureBox2.Image);
            }
            Graphics g = Graphics.FromImage(clone_img);
            Pen pen1 = new Pen(Color.Blue, 4);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            //Read file Existed User input
            String path = "D:\\605\\Source code\\UserInput\\input_" + noFile + ".txt";
            if (File.Exists(path))
            {
                foreach (var line in File.ReadLines(path))
                {
                    if (line == "")
                    {
                        break;
                    }

                    //Read line, and split it by whitespace into an array of strings
                    string[] tokens = line.Split();
                    //Parse element 0
                    double a = double.Parse(tokens[0]);
                    //Parse element 1
                    double b = double.Parse(tokens[1]);
                    //Draw
                    PointF new_point = new PointF((float)a, (float)b);
                    g.DrawRectangle(pen1, new_point.X, new_point.Y, 3, 3);
                    g.Save();
                }

                pictureBox2.Image = clone_img;
            }

            //Delete textbox
            outputText = "";
            richTextBox1.Text = "";

            //Show the desfault image
            pictureBox2.Image = mainImage;
        }
        private void button3_MouseClick(object sender, MouseEventArgs e)
        {
            outputText = "";
            richTextBox1.Text = "";
            //allow user to open jpg file 
            OpenFileDialog dlogOpen = new OpenFileDialog();
            dlogOpen.Filter = "Jpg Files|*.jpg";
            if (dlogOpen.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            currentFileName = dlogOpen.FileName;

            //open jpg file as Bitmap
            Bitmap img = (Bitmap)Bitmap.FromFile(dlogOpen.FileName);
            pictureBox1.Image = img;//set picture box image to UI
            mainImage = img;

            SetupStatZoomedImage();
            source_path = dlogOpen.FileName;

            Bitmap clone_img = new Bitmap(img);

            Graphics g = Graphics.FromImage(clone_img);
            Pen pen1 = new Pen(Color.Red, 4);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            //Read file Existed User input
            string noFile = currentFileName.Substring(Math.Max(0, currentFileName.Length - 6));
            noFile = noFile.Substring(0, 2);
            String path = "D:\\605\\Source code\\UserInput\\input_" + noFile + ".txt";
            if (File.Exists(path))
            {
                foreach (var line in File.ReadLines(path))
                {
                    if (line == "")
                    {
                        break;
                    }

                    //Read line, and split it by whitespace into an array of strings
                    string[] tokens = line.Split();
                    //Parse element 0
                    double a = double.Parse(tokens[0]);
                    //Parse element 1
                    double b = double.Parse(tokens[1]);
                    //Draw
                    PointF new_point = new PointF((float)a, (float)b);
                    g.DrawRectangle(pen1, new_point.X, new_point.Y, 3, 3);
                    g.Save();
                }

                pictureBox2.Image = clone_img;
            }

        } // end of button1_click
        private void open_btn_Click(object sender, EventArgs e)
        {
            outputText = "";
            richTextBox1.Text = "";
            //allow user to open jpg file 
            OpenFileDialog dlogOpen = new OpenFileDialog();
            dlogOpen.DefaultExt = ".avi";
            dlogOpen.Filter = "Media Files|*.mpg;*.avi;*.wma;*.mov;*.wav;*.mp2;*.mp3|All Files|*.*";
            if (dlogOpen.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            currentFileName = dlogOpen.FileName;

            Bitmap img = cvGui.GetThumnail(currentFileName);
            //pictureBox1.Image = new Bitmap("yourImage.jpg"); 

            pictureBox1.Image = img;//set processed image to picture box
            mainImage = img;

            SetupStatZoomedImage();

            //Dont delete
            //Bitmap processedImg = cvGui.ApplyFilter(img);//call opencv functions and get filterred image
            //pictureBox2.Image = processedImg;//set processed image to picture box
        }

        private void SetupStatZoomedImage()
        {
            //treat with zoom image
            double ratio_width = (double)mainImage.Width / pictureBox1.ClientSize.Width;
            double ratio_height = (double)mainImage.Height / pictureBox1.ClientSize.Height;
            paddingVertical = paddingHorizontal = 0;

            if (ratio_width >= ratio_height)
            {
                resizeFactor = ratio_width;
                double zoomedPictureBoxHeight = (double)pictureBox1.ClientSize.Height * resizeFactor;
                paddingVertical = (zoomedPictureBoxHeight - mainImage.Height) / 2;

            }
            else
            {
                resizeFactor = ratio_height;
                double zoomedPictureBoxWidth = (double)pictureBox1.ClientSize.Width * resizeFactor;
                paddingHorizontal = (zoomedPictureBoxWidth - mainImage.Width) / 2;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
            
                if (pictureBox1.ClientRectangle.Contains(pictureBox1.PointToClient(Control.MousePosition)))
                {
                    //painting = true;
                    Graphics g = Graphics.FromImage(mainImage);
                    Pen pen1 = new Pen(Color.Red, 4);
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    PointF new_point = new PointF((float)(e.X * resizeFactor - paddingHorizontal), (float)(e.Y * resizeFactor - paddingVertical));
                    userInput.Add(new_point);
                    g.DrawRectangle(pen1, new_point.X, new_point.Y, 3, 3);

                    g.Save();
                    pictureBox1.Image = mainImage;

                    richTextBox1.Text += new_point + "\n";
                    outputText += new_point.X + " " + new_point.Y + "\n";
                }
            }

        }

    }
}
