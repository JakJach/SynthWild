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
            short temp;

            int n = (int)(SampleRate / frequency);
            short stepSize = (short)(short.MaxValue * 2 / n);
            foreach (Oscillator oscillator in Controls.OfType<Oscillator>())
            {
                switch (oscillator.WaveForm)
                {
                    case EWaveFormType.Sine:
                        for (int i = 0; i < wave.Length; i++)
                            wave[i] = Convert.ToInt16(short.MaxValue * Math.Sin((Math.PI * 2 * frequency) / SampleRate * i));
                        break;
                    case EWaveFormType.Square:
                        for (int i = 0; i < wave.Length; i++)
                            wave[i] = Convert.ToInt16(short.MaxValue * Math.Sign(Math.Sin(Math.PI * 2 * frequency / SampleRate * i)));
                        break;
                    case EWaveFormType.Saw:
                        for (int i = 0; i < wave.Length; i++)
                        {
                            temp = -short.MaxValue;
                            for (int j = 0; j < n && i < SampleRate; j++)
                            {
                                temp += stepSize;
                                wave[i++] = Convert.ToInt16(temp);
                            }
                            i--;
                        }
                        break;
                    case EWaveFormType.Triangle:
                        temp = -short.MaxValue;
                        for(int i = 0; i< wave.Length; i++)
                        {
                            if (Math.Abs(temp + stepSize) > short.MaxValue)
                                stepSize = (short)-stepSize;
                            temp += stepSize;
                            wave[i] = Convert.ToInt16(temp);
                        }
                        break;
                    case EWaveFormType.Noise:
                        Random random = new Random();
                        for (int i = 0; i < wave.Length; i++)
                            wave[i] = (short)random.Next(short.MinValue, short.MaxValue);
                        break;
                }

            }
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
