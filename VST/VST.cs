using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VST
{
    public partial class VST : Form
    {
        private const int SampleRate = 44100;
        private const short BitsPerSample = 16;

        public VST()
        {
            InitializeComponent();
        }

        private void VST_KeyDown(object sender, KeyEventArgs e)
        {
            short[] wave = new short[SampleRate];
            byte[] binaryWave = new byte[SampleRate * sizeof(short)];
            float frequency = 440f;
            for (int i = 0; i < wave.Length; i++)
                wave[i] = Convert.ToInt16(short.MaxValue * Math.Sin((Math.PI * 2 * frequency)/SampleRate * i));
            Buffer.BlockCopy(wave, 0, binaryWave, 0, wave.Length * sizeof(short));
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using(BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
                {
                    int subChunkTwoSize = SampleRate * (BitsPerSample / 8);
                    binaryWriter.Write(new[] { 'R', 'I', 'F', 'F' });
                    binaryWriter.Write(36 + subChunkTwoSize);
                    binaryWriter.Write(new[] { 'W', 'A', 'V', 'E', 'f', 'm', 't', ' ' });
                    binaryWriter.Write(16);
                    binaryWriter.Write((short)1);
                    binaryWriter.Write((short)1);
                    binaryWriter.Write(SampleRate);
                    binaryWriter.Write(subChunkTwoSize);
                    binaryWriter.Write((short)(BitsPerSample / 8));
                    binaryWriter.Write(BitsPerSample);
                    binaryWriter.Write(new[] { 'd', 'a', 't', 'a' });
                    binaryWriter.Write(subChunkTwoSize);
                    binaryWriter.Write(binaryWave);
                    memoryStream.Position = 0;
                    new SoundPlayer(memoryStream).Play();
                };
            };
        }
    }
}
