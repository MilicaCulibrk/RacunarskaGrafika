// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2013.</copyright>
// <author>Srđan Mihić</author>
// <author>Aleksandar Josić</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using Assimp;
using System.IO;
using System.Reflection;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;
using SharpGL;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Threading;

namespace AssimpSample
{


    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {
        #region Atributi

        /// <summary>
        ///	 Ugao rotacije Meseca
        /// </summary>
        private float m_moonRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije Zemlje
        /// </summary>
        private float m_earthRotation = 0.0f;

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        private AssimpScene m_scene;

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        private float m_xRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        private float m_yRotation = 0.0f;

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        private float m_sceneDistance = 500.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;


        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;


        /// <summary>
        ///	 Scaling factor for model
        /// </summary>
        private float mm_scale = 1.0f;

        private enum TextureObjects { PODLOGA = 0, STEPENICE, OSOBA };
        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;
        private uint[] m_textures = null;
        private string[] m_textureFiles = { "..//..//Textures//plocice.jpg", "..//..//Textures//metal.jpg", "..//..//Textures//osoba.jpg" };

        private float[] m_ambientColor = { 0.2f, 0.2f, 0.2f, 0.2f };

        private float scaleUlaz = 140.0f;
        private float rotateY = 0.0f;

        private float plus = 0.0f;
        public float rAmbient = 0.3f;
        public float gAmbient = 0.3f;
        public float bAmbient = 0.3f;



        public float Plus { get => plus; set => plus = value; }

        public float ScaleUlaz { get => scaleUlaz; set => scaleUlaz = value; }
        public float RotateY { get => rotateY; set => rotateY = value; }

        public static bool startAnimation = false;
        public static DispatcherTimer timer;
        public static float x = -180f;
        public static float y = -205f;
        public static float z = -100f;
        public static float rot_y = 0f;
        public static bool endAnimation = false;
        public static bool disapper = false;

        /// <summary>
        /// Pomeranje brzine pingvina
        /// </summary>
        public static float brzinaPingvina = 10.0f;
        public static float skokPingvina = 200.0f;

        #endregion Atributi

        #region Properties

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        public AssimpScene Scene
        {
            get { return m_scene; }
            set { m_scene = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }
        public float MMScale
        {
            get { return mm_scale; }
            set { mm_scale = value; }
        }

        #endregion Properties

        #region Konstruktori

        /// <summary>
        ///  Konstruktor klase World.
        /// </summary>
        public World(String scenePath, String sceneFileName, int width, int height, OpenGL gl)
        {
            this.m_scene = new AssimpScene(scenePath, sceneFileName, gl);
            this.m_width = width;
            this.m_height = height;

            m_textures = new uint[m_textureCount];
        }

        /// <summary>
        ///  Destruktor klase World.
        /// </summary>
        ~World()
        {
            this.Dispose(false);
        }

        #endregion Konstruktori

        #region Metode

        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Color(1f, 0f, 0f);



            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE);
            //gl.Enable(OpenGL.GL_TEXTURE_2D);
            // gl.Enable(OpenGL.GL_LIGHTING);

            SetupRedLight(gl);
            SetupRedLight(gl);

            gl.Enable(OpenGL.GL_NORMALIZE);

            m_scene.LoadScene();
            m_scene.Initialize();
            LoadTextures(gl);
            SetupLighting(gl);
        }

        public void SetupLighting(OpenGL gl)
        {

            //color tracking - definisanje materijala
            //ambijentalna - sveprisutna svetlost iz svih pravaca, kao dnevna
            //difuzna - osvetljava objekte iz nekog pravca, refleksija pod normalnim uglom, stona lampa
           


            gl.Enable(OpenGL.GL_LIGHTING); //OMOGUCI SVETLO
            gl.Enable(OpenGL.GL_LIGHT0); //UKLJUCI SVETLO PORED STEPENICA
            //gl.Enable(OpenGL.GL_LIGHT1);// UKLJUCI SVETLO NA ANTENI

           // float[] RED = new float[] { 1.0f, 0f, 0f }; //CRVENA BOJA
                                                        //float[] WHITE = new float[] { 1.0f,1.0f,1f }; //BELA BOJA
                                                        //tackasti izvor, stacionaran
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f); //TACKASTI IZVOR
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, new float[] { 1f, 1f, 1f });//AKO IH STAVIM SVE NA 1, ONDA BUDE PREVISE BELA CELA SCENA
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, new float[] { 0.7f, 0.7f, 0.7f });
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, new float[] { 0.3f, 0.3f, 0.3f });

            float[] pos = { 3500, 500, 0f, 1.0f };
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, pos);
            //POZICIJU ZA STACIONARNO DEFINISEM OVDE KAKO KASNIJE NIJEDNA TRANSFORMACIJA NE BI UTICALA NA NJEGA


            //iznad hangara, cut off 25
            //gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 25f);//REFLEKTORSKI         
            //gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, RED);
            //gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPECULAR, RED);
            //POZICIJU ZA OVO SVETLO DEFINISEM POSLE, KOD CRTANJA ANTENE, JER TREBA UVEK DA BUDE NA VRHU ANTENE
    

        }

        private void SetupRedLight(OpenGL gl)
        {
            float[] light1pos = new float[] { -250.0f, 500.0f, -500.0f, 1.0f };
            float[] light1diffuse = new float[] { 1f, 0f, 0f, 1.0f };
            float[] light1ambient = new float[] { 0.2f, 0f, 0f, 1.0f };
            float[] light1direction = new float[] { 0.0f, -1.0f, 0.0f, 1.0f };

            //  gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, light1ambient);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPECULAR, light1diffuse);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, light1diffuse);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, light1direction);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 40.0f);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, light1pos);

            gl.Enable(OpenGL.GL_LIGHT1);
            gl.Enable(OpenGL.GL_LIGHTING);


        }

        private void LoadTextures(OpenGL gl)
        {
            //nacin stapanja teksture sa materijalom GL_ADD
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_ADD);
            gl.Enable(OpenGL.GL_TEXTURE_2D);

            gl.GenTextures(m_textureCount, m_textures);

            for (int i = 0; i < m_textureCount; ++i)
            {

                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);

                Bitmap image = new Bitmap(m_textureFiles[i]);
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                BitmapData imageData = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                //kreiraj teksturu
                gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, (int)OpenGL.GL_RGBA8, imageData.Width, imageData.Height, 0, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);


                //gl REPEAT po obema osama
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);

                //najblizi sused filtriranje
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);



                image.UnlockBits(imageData);
                image.Dispose();


            }
        }


            /// <summary>
            /// Podesava viewport i projekciju za OpenGL kontrolu.
            /// </summary>
            public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            gl.MatrixMode(OpenGL.GL_PROJECTION);      
            gl.LoadIdentity();
            gl.Perspective(50f, (double)width / height, 0.5f, 20000f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();            
        }


        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            // Ocisti sadrzaj kolor bafera i bafera dubine x
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.Enable(OpenGL.GL_DEPTH_TEST);

            gl.LoadIdentity();

            gl.LookAt(0f, 200.0f, m_sceneDistance, 0, 0, 0, 0.0f, 1.0f, 0.0f);
            float[] light0ambient = new float[] { rAmbient, gAmbient, bAmbient, 1.0f };
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);


            gl.PushMatrix();
            {

                // setting viewport to take full screen
                gl.Viewport(0, 0, m_width, m_height);

                
                gl.Translate(0.0f, 50.0f, -m_sceneDistance);
                gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
                gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);

                gl.PushMatrix();

                gl.Scale(1, mm_scale, 1);
               

                // person
                gl.PushMatrix();
                DrawModel(gl);
                gl.PopMatrix();

                gl.PopMatrix();

                // podloga
                gl.PushMatrix();
                DrawPodloga(gl);
                gl.PopMatrix();

                //stepenice
                gl.PushMatrix();
                DrawCube(gl);
                gl.PopMatrix();


            }
            gl.PopMatrix();

            // right bottom corner text
            gl.PushMatrix();
            {
                // placing text by redefining viewport
                DrawTextRightBottomCorner(gl);
            }
            gl.PopMatrix();

            // reset the viewport
            gl.Viewport(0, 0, m_width, m_height);

            gl.Flush();
        }




        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_scene.Dispose();
            }
        }

      

            private void DrawModel(OpenGL gl)
        {

            gl.PushMatrix();
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);

            
            gl.Translate(x, y, z);
            gl.Scale(2.5f, 2.5f, 2.5f);
            gl.Scale(1f, mm_scale, 1f);
            gl.Rotate(0.0f, 135f, 0);


            //gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, new float[] { 0.0f, -1.0f, 0.0f });
            //gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, new float[] { 5.0f, 50.0f, 0f, 1.0f });

            //gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, m_ambientColor);

            m_scene.Draw();

            gl.PopMatrix();
            gl.Flush();
        }

        private void DrawCube(OpenGL gl)
        {
            //prva stepenica
            gl.PushMatrix();
            gl.Rotate(0.0f, -35f, -0.0f);
            gl.Translate(-250, -274f, -40f);
            gl.Scale(150f, 20f, 30f);
            //gl.Color(1f, 0.5f, 1f);

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.STEPENICE]);
            
            Cube stepenica = new Cube();
            stepenica.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);


            //druga stepenica
            gl.Translate(0, 2f, -2f);
            stepenica.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);


            //treca stepenica
            gl.Translate(0, 2f, -2f);
            stepenica.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);

            //cetvrta stepenica
            gl.Translate(0, 2f, -2f);
            stepenica.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);

            //peta stepenica
            gl.Translate(0, 2f, -2f);
            stepenica.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);

            //sesta stepenica
            gl.Translate(0, 2f, -2f);
            stepenica.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);

            //sedma stepenica
            gl.Translate(0, -4, -2f);
            gl.Scale(1f, 7f, 1.5f);
            stepenica.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);

            gl.PopMatrix();
        }

    
        private void DrawPodloga(OpenGL gl)
        {
            float[] podloga =
            {
               2000.0f, -300.0f,  370.0f,
                2000.0f, -300.0f, -1800.0f,
                -500.0f, -300.0f, -1800.0F,
                -500.0f, -300.0f,  370.0f,


            };

            float[] textCoords =
            {
                1.0f, 0.0f,
                1.0f, 1.0f,
                0.0f, 1.0f,
                0.0f, 0.0f
            };

           // gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_ADD);

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.PODLOGA]);
            //gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_ADD);
            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.LoadIdentity();
            gl.Scale(1f, 1f, 1f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
          
            gl.Color(0.5f, 0.5f, 0.5f);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Normal(0, 1, 0);
            int i = 0;
            int j = 0;
            while (i < podloga.Length)
            {
                gl.TexCoord(textCoords[j], textCoords[j + 1]);
                gl.Vertex(podloga[i], podloga[i + 1], podloga[i + 2]);

                i += 3;
                j += 2;
            }

            gl.End();

        }

        private void DrawTextRightBottomCorner(OpenGL gl)
        {
        
            gl.Viewport((int)(m_width / 1.2), 0, m_width / 2, m_height / 2);
            gl.DrawText(160, 125, 1.0f, 0.0f, 0.0f, "Verdana Italic", 10, "Predmet: Racunarska grafika");
            gl.DrawText(160, 100, 1.0f, 0.0f, 0.0f, "Verdana Italic", 10, "Sk.god: 2019/20.");
            gl.DrawText(160, 75, 1.0f, 0.0f, 0.0f, "Verdana Italic", 10, "Ime: Milica");
            gl.DrawText(160, 50, 1.0f, 0.0f, 0.0f, "Verdana Italic", 10, "Prezime: Culibrk");
            gl.DrawText(160, 25, 1.0f, 0.0f, 0.0f, "Verdana Italic", 10, "Sifra zad: 11.1");

        }

        public static void Animacija(object sender, EventArgs e)
        {

            if (y == -205.0f)
            {
                timer.Start();
                y += 45f;
                z += -50f;
                x += 50f;

            }
            else if (y == -160.0f)
            {
                y += 42f;
                z += -47f;
                x += 50f;
            }
            else if (y == -118f)
            {
                y += 42f;
                z += -40f;
                x += 50f;
            }
            else if (y == -76f)
            {
                y += 40f;
                z += -30f;
                x += 50f;
            }
            else if (y == -36f)
            {
                y += 38f;
                z += -35f;
                x += 50f;
            }
            else if (y == 2f)
            {
                y += 38f;
                z += -35f;
                x += 50f;
            }
            else if (y == 40f)
            {
                y += 38f;
                z += -35f;
                x += 50f;
            }
            /*else if (x == -150f)
            {
                y -= 20f;
                x += 10f;
                rot_y = 10f;
            }
            else if (x == -160)
            {
                x += 10f;
                rot_y *= -1;
            }
            else if (x > -160f && x < 0f)
            {
                x += 10f;
                rot_y *= -1;
            } */
            else if (y == 130)
            {
                startAnimation = false;
                endAnimation = true;

               // x = 0f;
                //y = -100f;
                //rot_y = 0f;

                timer.Stop();
            }
        }


        #endregion Metode

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable metode
    }
}
