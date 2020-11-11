using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
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
            IEnumerable<Oscillator> oscillators = Controls.OfType<Oscillator>().Where(o => o.Status);

            short[] wave = new short[SampleRate];
            byte[] binaryWave = new byte[SampleRate * sizeof(short)];
            float frequency;
            int octave = 1;
            switch (e.KeyCode)
            {
                #region KeyCode Frequencies
                #region First Row
                case Keys.D1:
                case Keys.D4:
                case Keys.D8:
                case Keys.OemMinus:
                    frequency = 0f;
                    break;
                case Keys.D2:
                    frequency = octave * ENoteFrequency.Cis;
                    break;
                case Keys.D3:
                    frequency = octave * ENoteFrequency.Dis;
                    break;
                case Keys.D5:
                    frequency = octave * ENoteFrequency.Fis;
                    break;
                case Keys.D6:
                    frequency = octave * ENoteFrequency.Gis;
                    break;
                case Keys.D7:
                    frequency = octave * ENoteFrequency.Ais;
                    break;
                case Keys.D9:
                    frequency = 2 * octave * ENoteFrequency.Cis;
                    break;
                case Keys.D0:
                    frequency = 2 * octave * ENoteFrequency.Dis;
                    break;
                case Keys.Oemplus:
                    frequency = 2 * octave * ENoteFrequency.Fis;
                    break;
                #endregion
                #region Second Row
                case Keys.Q:
                    frequency = octave * ENoteFrequency.C;
                    break;
                case Keys.W:
                    frequency = octave * ENoteFrequency.D;
                    break;
                case Keys.E:
                    frequency = octave * ENoteFrequency.E;
                    break;
                case Keys.R:
                    frequency = octave * ENoteFrequency.F;
                    break;
                case Keys.T:
                    frequency = octave * ENoteFrequency.G;
                    break;
                case Keys.Y:
                    frequency = octave * ENoteFrequency.A;
                    break;
                case Keys.U:
                    frequency = octave * ENoteFrequency.B;
                    break;
                case Keys.I:
                    frequency = 2 * octave * ENoteFrequency.C;
                    break;
                case Keys.O:
                    frequency = 2 * octave * ENoteFrequency.D;
                    break;
                case Keys.P:
                    frequency = 2 * octave * ENoteFrequency.E;
                    break;
                case Keys.OemOpenBrackets:
                    frequency = 2 * octave * ENoteFrequency.F;
                    break;
                case Keys.OemCloseBrackets:
                    frequency = 2 * octave * ENoteFrequency.G;
                    break;
                #endregion
                #region Third Row
                case Keys.A:
                case Keys.F:
                case Keys.K:
                    frequency = 0f;
                    break;
                case Keys.S:
                    frequency = octave * ENoteFrequency.Cis / 2;
                    break;
                case Keys.D:
                    frequency = octave * ENoteFrequency.Dis / 2;
                    break;
                case Keys.G:
                    frequency = octave * ENoteFrequency.Fis / 2;
                    break;
                case Keys.H:
                    frequency = octave * ENoteFrequency.Gis / 2;
                    break;
                case Keys.J:
                    frequency = octave * ENoteFrequency.Ais / 2;
                    break;
                case Keys.L:
                    frequency = octave * ENoteFrequency.Cis;
                    break;
                case Keys.OemSemicolon:
                    frequency = octave * ENoteFrequency.Dis;
                    break;
                #endregion
                #region Fourth Row
                case Keys.Z:
                    frequency = octave * ENoteFrequency.C / 2;
                    break;
                case Keys.X:
                    frequency = octave * ENoteFrequency.D / 2;
                    break;
                case Keys.C:
                    frequency = octave * ENoteFrequency.E / 2;
                    break;
                case Keys.V:
                    frequency = octave * ENoteFrequency.F / 2;
                    break;
                case Keys.B:
                    frequency = octave * ENoteFrequency.G / 2;
                    break;
                case Keys.N:
                    frequency = octave * ENoteFrequency.A / 2;
                    break;
                case Keys.M:
                    frequency = octave * ENoteFrequency.B / 2;
                    break;
                case Keys.Oemcomma:
                    frequency = octave * ENoteFrequency.C;
                    break;
                case Keys.OemPeriod:
                    frequency = octave * ENoteFrequency.D;
                    break;
                case Keys.OemQuestion:
                    frequency = octave * ENoteFrequency.E;
                    break;
                #endregion
                #endregion
                default:
                    frequency = 0f;
                    break;
            }
            short temp;
            int n = (int)(SampleRate / frequency);
            short stepSize = (short)(short.MaxValue * 2 / n);
            foreach (Oscillator oscillator in oscillators)
            {
                switch (oscillator.WaveForm)
                {
                    case EWaveFormType.Sine:
                        for (int i = 0; i < wave.Length; i++)
                            wave[i] += Convert.ToInt16(short.MaxValue * Math.Sin((Math.PI * 2 * frequency) / SampleRate * i) / oscillators.Count());
                        break;
                    case EWaveFormType.Square:
                        for (int i = 0; i < wave.Length; i++)
                            wave[i] += Convert.ToInt16(short.MaxValue * Math.Sign(Math.Sin(Math.PI * 2 * frequency / SampleRate * i)) / oscillators.Count());
                        break;
                    case EWaveFormType.Saw:
                        for (int i = 0; i < wave.Length; i++)
                        {
                            temp = -short.MaxValue;
                            for (int j = 0; j < n && i < SampleRate; j++)
                            {
                                temp += stepSize;
                                wave[i++] += Convert.ToInt16(temp / oscillators.Count());
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
                            wave[i] += Convert.ToInt16(temp / oscillators.Count());
                        }
                        break;
                    case EWaveFormType.Noise:
                        Random random = new Random();
                        for (int i = 0; i < wave.Length; i++)
                            wave[i] += Convert.ToInt16(random.Next(short.MinValue, short.MaxValue) / oscillators.Count());
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
