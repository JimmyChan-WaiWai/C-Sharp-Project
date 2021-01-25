using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;


namespace Robocon_2019_01
{
    public partial class Form1 : Form
    {
        string m_Final_Data_Message_01 = "";
        string m_Final_Data_Message_02 = "";

        byte m_Motor_ID_01 = 1;
        byte[] m_Motor_ID = new byte[8] {1, 2, 3, 4, 5, 6, 7, 8};


        byte[] m_Final_Data_List = new byte[100];
        int m_Final_Data_List_Count = 0;
        int m_Final_Data_Status = 0;
        byte boxnumber = 1;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            m_Motor_ID_01 = byte.Parse(m_Text_Motor_ID_01.Text);
            m_SerialPort_01.Open();

        }
        private void f_Motor_Action(int t_Motor_ID, int t_Motor_Action_Position, int t_Motor_Action_Time, int t_Motor_Command_Delay)
        {
            if (t_Motor_Action_Position <= 1000)
            {
                byte[] m_Data_Send = this.Motor_Position_Set((byte)t_Motor_ID, t_Motor_Action_Position, t_Motor_Action_Time);
                m_SerialPort_01.Write(m_Data_Send, 0, m_Data_Send.Length);
                Thread.Sleep(t_Motor_Command_Delay);
            }
            else if (t_Motor_Action_Position == 9990)
            {
                // Motor Disable
                byte[] m_Data_Send = Motor_Enable((byte)t_Motor_ID, 0);
                m_SerialPort_01.Write(m_Data_Send, 0, m_Data_Send.Length);
            }
            else if (t_Motor_Action_Position == 9991)
            {
                // Motor Enable
                byte[] m_Data_Send = Motor_Enable((byte)t_Motor_ID, 1);
                m_SerialPort_01.Write(m_Data_Send, 0, m_Data_Send.Length);
            }
        }
        private void m_SerialPort_01_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            int RX_Data_Byte_01;
            int RX_Data_Length_01 = m_SerialPort_01.BytesToRead;
            for (int Temp_Count = 0; Temp_Count < RX_Data_Length_01; Temp_Count++)
            {
                RX_Data_Byte_01 = m_SerialPort_01.ReadByte();
                m_Final_Data_List_Count++;
                m_Final_Data_List[m_Final_Data_List_Count] = (byte)RX_Data_Byte_01;
            }
        }

        private void m_Text_Motor_ID_01_TextChanged(object sender, EventArgs e)
        {
            if (!m_Text_Motor_ID_01.Text.Equals(""))
            {
                m_Motor_ID_01 = byte.Parse(m_Text_Motor_ID_01.Text);
            }
        }

        private void m_Timer_01_Tick(object sender, EventArgs e)
        {

        }

        byte[] Motor_Enable(byte Temp_Motor_ID_01, byte Temp_Enable_01)
        {
            byte[] Temp_CMD_01 = new byte[7];
            Temp_CMD_01[0] = 0x55;
            Temp_CMD_01[1] = 0x55;
            Temp_CMD_01[2] = Temp_Motor_ID_01;
            Temp_CMD_01[3] = 4;
            Temp_CMD_01[4] = 31;
            Temp_CMD_01[5] = Temp_Enable_01;
            Temp_CMD_01[6] = Motor_CMD_CheckSum(Temp_CMD_01);

            return Temp_CMD_01;
        }

        byte[] Motor_Position_Get(byte Temp_Motor_ID_01)
        {
            byte[] Temp_CMD_01 = new byte[6];
            Temp_CMD_01[0] = 0x55;
            Temp_CMD_01[1] = 0x55;
            Temp_CMD_01[2] = Temp_Motor_ID_01;
            Temp_CMD_01[3] = 3;
            Temp_CMD_01[4] = 28;
            Temp_CMD_01[5] = Motor_CMD_CheckSum(Temp_CMD_01);

            return Temp_CMD_01;
        }

        byte[] Motor_Position_Set(byte Temp_Motor_ID_01, int Temp_Motor_Position_01, int Temp_Motor_ActionTime_01)
        {
            if (Temp_Motor_Position_01 < 0)
                Temp_Motor_Position_01 = 0;
            if (Temp_Motor_Position_01 > 1000)
                Temp_Motor_Position_01 = 1000;

            byte[] Temp_CMD_01 = new byte[10];            
            Temp_CMD_01[0] = 0x55;
            Temp_CMD_01[1] = 0x55;
            Temp_CMD_01[2] = Temp_Motor_ID_01;
            Temp_CMD_01[3] = 7;
            Temp_CMD_01[4] = 1;
            Temp_CMD_01[5] = (byte)((Temp_Motor_Position_01));
            Temp_CMD_01[6] = (byte)((Temp_Motor_Position_01) >> 8);
            Temp_CMD_01[7] = (byte)((Temp_Motor_ActionTime_01));
            Temp_CMD_01[8] = (byte)((Temp_Motor_ActionTime_01) >> 8);
            Temp_CMD_01[9] = Motor_CMD_CheckSum(Temp_CMD_01);

            return Temp_CMD_01;
        }

        byte[] Motor_ID_Set(byte Temp_Motor_ID_01_OLD, byte Temp_Motor_ID_02_NEW)
        {
            // 254 - All Motor
            byte[] Temp_CMD_01 = new byte[7];

            Temp_CMD_01[0] = 0x55;
            Temp_CMD_01[1] = 0x55;
            Temp_CMD_01[2] = Temp_Motor_ID_01_OLD;
            Temp_CMD_01[3] = 4;
            Temp_CMD_01[4] = 13;
            Temp_CMD_01[5] = Temp_Motor_ID_02_NEW;
            Temp_CMD_01[6] = Motor_CMD_CheckSum(Temp_CMD_01);

            return Temp_CMD_01;
        }

        byte Motor_CMD_CheckSum(byte[] Temp_CMD_01)
        {
            int  Temp_Value_01 = 0;
            byte Temp_Value_02;
            for (Temp_Value_02 = 2; Temp_Value_02 < Temp_CMD_01[3] + 2; Temp_Value_02++)
            {
                Temp_Value_01 += Temp_CMD_01[Temp_Value_02];
            }
            Temp_Value_01 = ~Temp_Value_01;
            Temp_Value_02 = (byte)Temp_Value_01;
            return Temp_Value_02;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Position Set 100
            byte[] m_Data_Send_01 = Motor_Position_Set(m_Motor_ID_01, 300, 0);
            m_SerialPort_01.Write(m_Data_Send_01, 0, m_Data_Send_01.Length);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Position Set 500
            byte[] m_Data_Send_01 = Motor_Position_Set(m_Motor_ID_01, 500, 0);
            m_SerialPort_01.Write(m_Data_Send_01, 0, m_Data_Send_01.Length);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Position Set 900
            byte[] m_Data_Send_01 = Motor_Position_Set(m_Motor_ID_01, 700, 0);
            m_SerialPort_01.Write(m_Data_Send_01, 0, m_Data_Send_01.Length);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Motor Enable
            byte[] m_Data_Send_01 = Motor_Enable(m_Motor_ID_01, 1);
            m_SerialPort_01.Write(m_Data_Send_01, 0, m_Data_Send_01.Length);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Motor Disable
            byte[] m_Data_Send_01 = Motor_Enable(m_Motor_ID_01, 0);
            m_SerialPort_01.Write(m_Data_Send_01, 0, m_Data_Send_01.Length);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            
                m_Final_Data_List_Count = 0;
                byte[] m_Data_Send_01 = Motor_Position_Get(m_Motor_ID_01);
                m_SerialPort_01.Write(m_Data_Send_01, 0, m_Data_Send_01.Length);
            
        }

        private void button7_Click(object sender, EventArgs e)
        {
            m_Final_Data_Message_01 =
                //"0x" + m_Final_Data_List[0].ToString("X2") +
                "0x" + m_Final_Data_List[1].ToString("X2") +
                ", 0x" + m_Final_Data_List[2].ToString("X2") +
                ", 0x" + m_Final_Data_List[3].ToString("X2") +
                ", 0x" + m_Final_Data_List[4].ToString("X2") +
                ", 0x" + m_Final_Data_List[5].ToString("X2") +
                ", 0x" + m_Final_Data_List[6].ToString("X2") +
                ", 0x" + m_Final_Data_List[7].ToString("X2") +
                ", 0x" + m_Final_Data_List[8].ToString("X2");
                //", 0x" + m_Final_Data_List[9].ToString("X2");
            m_MessageBox_01.Text = m_Final_Data_Message_01;
            m_Final_Data_Message_01 = "";


            int Temp_Value_01 = m_Final_Data_List[7] * 0x100 + m_Final_Data_List[6];
            m_Final_Data_Message_02 = Temp_Value_01.ToString("d4");
            m_MessageBox_02.Text = m_Final_Data_Message_02;
            boxnumber = m_Motor_ID_01;
            switch (boxnumber)
            {
                case 1: textBox1.Text = m_Final_Data_Message_02;break;
                case 2: textBox2.Text = m_Final_Data_Message_02; break;
                case 3: textBox3.Text = m_Final_Data_Message_02; break;
                case 4: textBox4.Text = m_Final_Data_Message_02; break;
                case 5: textBox5.Text = m_Final_Data_Message_02; break;
                case 6: textBox6.Text = m_Final_Data_Message_02; break;
                case 7: textBox7.Text = m_Final_Data_Message_02; break;
                case 8: textBox8.Text = m_Final_Data_Message_02; break;
            }

            m_Final_Data_Message_02 = "";
        }

       
       

        private void button8_Click(object sender, EventArgs e)
        {
            
            if (!m_Text_Motor_ID_02.Text.Equals(""))
            {
                byte Temp_Motor_ID_01 = byte.Parse(m_Text_Motor_ID_02.Text);
                byte[] m_Data_Send_01 = Motor_ID_Set(254, Temp_Motor_ID_01);
                m_SerialPort_01.Write(m_Data_Send_01, 0, m_Data_Send_01.Length);
            }
            /*
            */
        }

        private void button9_Click(object sender, EventArgs e)
        {
            // Position 13 -> 500
            byte[] m_Data_Send_13 = Motor_Position_Set(13, 500, 3000);
            m_SerialPort_01.Write(m_Data_Send_13, 0, m_Data_Send_13.Length);
            Thread.Sleep(50);            
            // Position 14 -> 200
            byte[] m_Data_Send_14 = Motor_Position_Set(14, 200, 3000);
            m_SerialPort_01.Write(m_Data_Send_14, 0, m_Data_Send_14.Length);
            Thread.Sleep(50);            
            // Position 15 -> 100
            byte[] m_Data_Send_15 = Motor_Position_Set(15, 100, 3000);
            m_SerialPort_01.Write(m_Data_Send_15, 0, m_Data_Send_15.Length);
            Thread.Sleep(50);            
            // Position 16 -> 850
            byte[] m_Data_Send_16 = Motor_Position_Set(16, 900, 3000);
            m_SerialPort_01.Write(m_Data_Send_16, 0, m_Data_Send_16.Length);
            Thread.Sleep(50);            
        }

        private void button10_Click(object sender, EventArgs e)
        {
            // Position 13 -> 500
            byte[] m_Data_Send_13 = Motor_Position_Set(13, 500, 3000);
            m_SerialPort_01.Write(m_Data_Send_13, 0, m_Data_Send_13.Length);
            Thread.Sleep(50);
            // Position 14 -> 200
            byte[] m_Data_Send_14 = Motor_Position_Set(14, 400, 3000);
            m_SerialPort_01.Write(m_Data_Send_14, 0, m_Data_Send_14.Length);
            Thread.Sleep(50);
            // Position 15 -> 100
            byte[] m_Data_Send_15 = Motor_Position_Set(15, 100, 3000);
            m_SerialPort_01.Write(m_Data_Send_15, 0, m_Data_Send_15.Length);
            Thread.Sleep(50);
            // Position 16 -> 850
            byte[] m_Data_Send_16 = Motor_Position_Set(16, 900, 3000);
            m_SerialPort_01.Write(m_Data_Send_16, 0, m_Data_Send_16.Length);
            Thread.Sleep(50);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            // Position 01 -> 500
            byte[] m_Data_Send_01 = Motor_Position_Set(1, 150, 3000);
            m_SerialPort_01.Write(m_Data_Send_01, 0, m_Data_Send_01.Length);
            Thread.Sleep(50);
            // Position 04 -> 200
            byte[] m_Data_Send_04 = Motor_Position_Set(4, 850, 3000);
            m_SerialPort_01.Write(m_Data_Send_04, 0, m_Data_Send_04.Length);
            Thread.Sleep(50);
            // Position 07 -> 100
            byte[] m_Data_Send_07 = Motor_Position_Set(7, 150, 3000);
            m_SerialPort_01.Write(m_Data_Send_07, 0, m_Data_Send_07.Length);
            Thread.Sleep(50);
            // Position 10 -> 850
            byte[] m_Data_Send_10 = Motor_Position_Set(10, 850, 3000);
            m_SerialPort_01.Write(m_Data_Send_10, 0, m_Data_Send_10.Length);
            Thread.Sleep(50);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            // Position 02 -> 500
            byte[] m_Data_Send_02 = Motor_Position_Set(2, 700, 3000);
            m_SerialPort_01.Write(m_Data_Send_02, 0, m_Data_Send_02.Length);
            Thread.Sleep(50);
            // Position 05 -> 500
            byte[] m_Data_Send_05 = Motor_Position_Set(5, 700, 3000);
            m_SerialPort_01.Write(m_Data_Send_05, 0, m_Data_Send_05.Length);
            Thread.Sleep(50);
            // Position 08 -> 500
            byte[] m_Data_Send_08 = Motor_Position_Set(8, 700, 3000);
            m_SerialPort_01.Write(m_Data_Send_08, 0, m_Data_Send_08.Length);
            Thread.Sleep(50);
            // Position 11 -> 500
            byte[] m_Data_Send_11 = Motor_Position_Set(11, 700, 3000);
            m_SerialPort_01.Write(m_Data_Send_11, 0, m_Data_Send_11.Length);
            Thread.Sleep(50);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            // Position 03 -> 500
            byte[] m_Data_Send_03 = Motor_Position_Set(3, 700, 3000);
            m_SerialPort_01.Write(m_Data_Send_03, 0, m_Data_Send_03.Length);
            Thread.Sleep(50);
            // Position 06 -> 500
            byte[] m_Data_Send_06 = Motor_Position_Set(6, 700, 3000);
            m_SerialPort_01.Write(m_Data_Send_06, 0, m_Data_Send_06.Length);
            Thread.Sleep(50);
            // Position 09 -> 500
            byte[] m_Data_Send_09 = Motor_Position_Set(9, 700, 3000);
            m_SerialPort_01.Write(m_Data_Send_09, 0, m_Data_Send_09.Length);
            Thread.Sleep(50);
            // Position 12 -> 500
            byte[] m_Data_Send_12 = Motor_Position_Set(12, 700, 3000);
            m_SerialPort_01.Write(m_Data_Send_12, 0, m_Data_Send_12.Length);
            Thread.Sleep(50);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            // Position 02 -> 500
            byte[] m_Data_Send_02 = Motor_Position_Set(2, 600, 3000);
            m_SerialPort_01.Write(m_Data_Send_02, 0, m_Data_Send_02.Length);
            Thread.Sleep(50);
            // Position 05 -> 500
            byte[] m_Data_Send_05 = Motor_Position_Set(5, 600, 3000);
            m_SerialPort_01.Write(m_Data_Send_05, 0, m_Data_Send_05.Length);
            Thread.Sleep(50);
            // Position 08 -> 500
            byte[] m_Data_Send_08 = Motor_Position_Set(8, 600, 3000);
            m_SerialPort_01.Write(m_Data_Send_08, 0, m_Data_Send_08.Length);
            Thread.Sleep(50);
            // Position 11 -> 500
            byte[] m_Data_Send_11 = Motor_Position_Set(11, 600, 3000);
            m_SerialPort_01.Write(m_Data_Send_11, 0, m_Data_Send_11.Length);
            Thread.Sleep(50);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            // Position 08 -> 500
            // 100 IN
            // 900 OUT
            byte[] m_Data_Send_08 = Motor_Position_Set(8, 700, 3000);
            m_SerialPort_01.Write(m_Data_Send_08, 0, m_Data_Send_08.Length);
            Thread.Sleep(50);

            // Position 09 -> 500
            // 100 IN
            // 900 OUT
            byte[] m_Data_Send_09 = Motor_Position_Set(9, 800, 3000);
            m_SerialPort_01.Write(m_Data_Send_09, 0, m_Data_Send_09.Length);
            Thread.Sleep(50);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            // Position 08 -> 500
            // 100 IN
            // 900 OUT
            byte[] m_Data_Send_08 = Motor_Position_Set(8, 600, 3000);
            m_SerialPort_01.Write(m_Data_Send_08, 0, m_Data_Send_08.Length);
            Thread.Sleep(50);

            // Position 09 -> 500
            // 100 IN
            // 900 OUT
            byte[] m_Data_Send_09 = Motor_Position_Set(9, 700, 3000);
            m_SerialPort_01.Write(m_Data_Send_09, 0, m_Data_Send_09.Length);
            Thread.Sleep(50);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            
            int m_F_L_Middle = 750;
            int m_F_R_Middle = 750;
            int m_F_L_Gound  = 300;
            int m_F_R_Gound  = 300;

            int m_B_L_Middle = 600;
            int m_B_R_Middle = 600;
            int m_B_L_Gound = 200;
            int m_B_R_Gound = 200;

            int m_F_L_Middle_Time = 2000;
            int m_F_R_Middle_Time = 2000;
            int m_F_L_Gound_Time  = 2000;
            int m_F_R_Gound_Time  = 2000;

            int m_B_L_Middle_Time = 2000;
            int m_B_R_Middle_Time = 2000;
            int m_B_L_Gound_Time  = 2000;
            int m_B_R_Gound_Time  = 2000;

            //============================
            // Front     0
            // Middel  500
            // Back   1000
            //============================

            //======================================================================
            // FFF
            // Position 05 -> 500
            // IN  900
            // OUT 100
            m_F_L_Middle = 1000 - m_F_L_Middle;
            byte[] m_Data_Send_05 = Motor_Position_Set(5, m_F_L_Middle, m_F_L_Middle_Time);
            m_SerialPort_01.Write(m_Data_Send_05, 0, m_Data_Send_05.Length);
            Thread.Sleep(50);
            // Position 08 -> 500
            // IN  900
            // OUT 100
            m_F_R_Middle = 1000 - m_F_R_Middle;
            byte[] m_Data_Send_08 = Motor_Position_Set(8, m_F_R_Middle, m_F_R_Middle_Time);
            m_SerialPort_01.Write(m_Data_Send_08, 0, m_Data_Send_08.Length);
            Thread.Sleep(50);
            // Position 06 -> 500
            // IN  100
            // OUT 900
            byte[] m_Data_Send_06 = Motor_Position_Set(6, m_F_L_Gound, m_F_L_Gound_Time);
            m_SerialPort_01.Write(m_Data_Send_06, 0, m_Data_Send_06.Length);
            Thread.Sleep(50);
            // Position 09 -> 500
            // IN  100
            // OUT 900
            byte[] m_Data_Send_09 = Motor_Position_Set(9, m_F_R_Gound, m_F_R_Gound_Time);
            m_SerialPort_01.Write(m_Data_Send_09, 0, m_Data_Send_09.Length);
            Thread.Sleep(50);
            //======================================================================
            // BBB
            // Position 02 -> 500
            // IN  100
            // OUT 900
            byte[] m_Data_Send_02 = Motor_Position_Set(2, m_B_L_Middle, m_B_L_Middle_Time);
            m_SerialPort_01.Write(m_Data_Send_02, 0, m_Data_Send_02.Length);
            Thread.Sleep(50);
            // Position 11 -> 500
            // IN  100
            // OUT 900
            byte[] m_Data_Send_11 = Motor_Position_Set(11, m_B_R_Middle, m_B_R_Middle_Time);
            m_SerialPort_01.Write(m_Data_Send_11, 0, m_Data_Send_11.Length);
            Thread.Sleep(50);
            // Position 03 -> 500
            // IN  900
            // OUT 100
            m_B_L_Gound = 1000 - m_B_L_Gound;
            byte[] m_Data_Send_03 = Motor_Position_Set(3, m_B_L_Gound, m_B_L_Gound_Time);
            m_SerialPort_01.Write(m_Data_Send_03, 0, m_Data_Send_03.Length);
            Thread.Sleep(50);
            // Position 12 -> 500
            // IN  900
            // OUT 100
            m_B_R_Gound = 1000 - m_B_R_Gound;
            byte[] m_Data_Send_12 = Motor_Position_Set(12, m_B_R_Gound, m_B_R_Gound_Time);
            m_SerialPort_01.Write(m_Data_Send_12, 0, m_Data_Send_12.Length);
            Thread.Sleep(50);
            //======================================================================
        }

        private void button18_Click(object sender, EventArgs e)
        {
            int m_F_L_Middle = 700;
            int m_F_R_Middle = 700;
            int m_F_L_Gound = 200;
            int m_F_R_Gound = 200;

            int m_B_L_Middle = 550;
            int m_B_R_Middle = 550;
            int m_B_L_Gound = 250;
            int m_B_R_Gound = 250;

            int m_F_L_Middle_Time = 2000;
            int m_F_R_Middle_Time = 2000;
            int m_F_L_Gound_Time = 2000;
            int m_F_R_Gound_Time = 2000;

            int m_B_L_Middle_Time = 2000;
            int m_B_R_Middle_Time = 2000;
            int m_B_L_Gound_Time = 2000;
            int m_B_R_Gound_Time = 2000;

            //============================
            // Front     0
            // Middel  500
            // Back   1000
            //============================

            //======================================================================
            // FFF
            // Position 05 -> 500
            // IN  900
            // OUT 100
            m_F_L_Middle = 1000 - m_F_L_Middle;
            byte[] m_Data_Send_05 = Motor_Position_Set(5, m_F_L_Middle, m_F_L_Middle_Time);
            m_SerialPort_01.Write(m_Data_Send_05, 0, m_Data_Send_05.Length);
            Thread.Sleep(50);
            // Position 08 -> 500
            // IN  900
            // OUT 100
            m_F_R_Middle = 1000 - m_F_R_Middle;
            byte[] m_Data_Send_08 = Motor_Position_Set(8, m_F_R_Middle, m_F_R_Middle_Time);
            m_SerialPort_01.Write(m_Data_Send_08, 0, m_Data_Send_08.Length);
            Thread.Sleep(50);
            // Position 06 -> 500
            // IN  100
            // OUT 900
            byte[] m_Data_Send_06 = Motor_Position_Set(6, m_F_L_Gound, m_F_L_Gound_Time);
            m_SerialPort_01.Write(m_Data_Send_06, 0, m_Data_Send_06.Length);
            Thread.Sleep(50);
            // Position 09 -> 500
            // IN  100
            // OUT 900
            byte[] m_Data_Send_09 = Motor_Position_Set(9, m_F_R_Gound, m_F_R_Gound_Time);
            m_SerialPort_01.Write(m_Data_Send_09, 0, m_Data_Send_09.Length);
            Thread.Sleep(50);
            //======================================================================
            // BBB
            // Position 02 -> 500
            // IN  100
            // OUT 900
            byte[] m_Data_Send_02 = Motor_Position_Set(2, m_B_L_Middle, m_B_L_Middle_Time);
            m_SerialPort_01.Write(m_Data_Send_02, 0, m_Data_Send_02.Length);
            Thread.Sleep(50);
            // Position 11 -> 500
            // IN  100
            // OUT 900
            byte[] m_Data_Send_11 = Motor_Position_Set(11, m_B_R_Middle, m_B_R_Middle_Time);
            m_SerialPort_01.Write(m_Data_Send_11, 0, m_Data_Send_11.Length);
            Thread.Sleep(50);
            // Position 03 -> 500
            // IN  900
            // OUT 100
            m_B_L_Gound = 1000 - m_B_L_Gound;
            byte[] m_Data_Send_03 = Motor_Position_Set(3, m_B_L_Gound, m_B_L_Gound_Time);
            m_SerialPort_01.Write(m_Data_Send_03, 0, m_Data_Send_03.Length);
            Thread.Sleep(50);
            // Position 12 -> 500
            // IN  900
            // OUT 100
            m_B_R_Gound = 1000 - m_B_R_Gound;
            byte[] m_Data_Send_12 = Motor_Position_Set(12, m_B_R_Gound, m_B_R_Gound_Time);
            m_SerialPort_01.Write(m_Data_Send_12, 0, m_Data_Send_12.Length);
            Thread.Sleep(50);
            //======================================================================
        }

        private void button19_Click(object sender, EventArgs e)
        {
            int m_F_L_Middle = 700;
            int m_F_R_Middle = 900;
            int m_F_L_Gound = 400;
            int m_F_R_Gound = 100;

            int m_B_L_Middle = 600;
            int m_B_R_Middle = 600;
            int m_B_L_Gound = 150;
            int m_B_R_Gound = 200;

            int m_F_L_Middle_Time = 2000;
            int m_F_R_Middle_Time = 2000;
            int m_F_L_Gound_Time = 2000;
            int m_F_R_Gound_Time = 2000;

            int m_B_L_Middle_Time = 2000;
            int m_B_R_Middle_Time = 2000;
            int m_B_L_Gound_Time = 500;
            int m_B_R_Gound_Time = 2000;

            //============================
            // Front     0
            // Middel  500
            // Back   1000
            //============================

            // Position 03 -> 500
            // IN  900
            // OUT 100
            m_B_L_Gound = 1000 - m_B_L_Gound;
            byte[] m_Data_Send_03 = Motor_Position_Set(3, m_B_L_Gound, m_B_L_Gound_Time);
            m_SerialPort_01.Write(m_Data_Send_03, 0, m_Data_Send_03.Length);
            Thread.Sleep(50);
            //======================================================================
            // FFF
            // Position 05 -> 500
            // IN  900
            // OUT 100
            m_F_L_Middle = 1000 - m_F_L_Middle;
            byte[] m_Data_Send_05 = Motor_Position_Set(5, m_F_L_Middle, m_F_L_Middle_Time);
            m_SerialPort_01.Write(m_Data_Send_05, 0, m_Data_Send_05.Length);
            Thread.Sleep(50);
            
            // Position 08 -> 500
            // IN  900
            // OUT 100
            m_F_R_Middle = 1000 - m_F_R_Middle;
            byte[] m_Data_Send_08 = Motor_Position_Set(8, m_F_R_Middle, m_F_R_Middle_Time);
            m_SerialPort_01.Write(m_Data_Send_08, 0, m_Data_Send_08.Length);
            Thread.Sleep(50);
            // Position 06 -> 500
            // IN  100
            // OUT 900
            byte[] m_Data_Send_06 = Motor_Position_Set(6, m_F_L_Gound, m_F_L_Gound_Time);
            m_SerialPort_01.Write(m_Data_Send_06, 0, m_Data_Send_06.Length);
            Thread.Sleep(50);
            // Position 09 -> 500
            // IN  100
            // OUT 900
            byte[] m_Data_Send_09 = Motor_Position_Set(9, m_F_R_Gound, m_F_R_Gound_Time);
            m_SerialPort_01.Write(m_Data_Send_09, 0, m_Data_Send_09.Length);
            Thread.Sleep(50);
            //======================================================================
            // BBB
            // Position 02 -> 500
            // IN  100
            // OUT 900
            byte[] m_Data_Send_02 = Motor_Position_Set(2, m_B_L_Middle, m_B_L_Middle_Time);
            m_SerialPort_01.Write(m_Data_Send_02, 0, m_Data_Send_02.Length);
            Thread.Sleep(50);
            // Position 11 -> 500
            // IN  100
            // OUT 900
            byte[] m_Data_Send_11 = Motor_Position_Set(11, m_B_R_Middle, m_B_R_Middle_Time);
            m_SerialPort_01.Write(m_Data_Send_11, 0, m_Data_Send_11.Length);
            Thread.Sleep(50);
            // Position 03 -> 500
            // IN  900
            // OUT 100
            //m_B_L_Gound = 1000 - m_B_L_Gound;
            //byte[] m_Data_Send_03 = Motor_Position_Set(3, m_B_L_Gound, m_B_L_Gound_Time);
            //m_SerialPort_01.Write(m_Data_Send_03, 0, m_Data_Send_03.Length);
            //Thread.Sleep(50);
            // Position 12 -> 500
            // IN  900
            // OUT 100
            m_B_R_Gound = 1000 - m_B_R_Gound;
            byte[] m_Data_Send_12 = Motor_Position_Set(12, m_B_R_Gound, m_B_R_Gound_Time);
            m_SerialPort_01.Write(m_Data_Send_12, 0, m_Data_Send_12.Length);
            Thread.Sleep(50);
            //======================================================================
        }

        private void button20_Click(object sender, EventArgs e)
        {
            int m_F_L_Middle = 700;
            int m_F_R_Middle = 400;
            int m_F_L_Gound = 400;
            int m_F_R_Gound = 800;

            int m_B_L_Middle = 600;
            int m_B_R_Middle = 600;
            int m_B_L_Gound = 150;
            int m_B_R_Gound = 200;

            int m_F_L_Middle_Time = 2000;
            int m_F_R_Middle_Time = 2000;
            int m_F_L_Gound_Time = 2000;
            int m_F_R_Gound_Time = 2000;

            int m_B_L_Middle_Time = 2000;
            int m_B_R_Middle_Time = 2000;
            int m_B_L_Gound_Time = 2000;
            int m_B_R_Gound_Time = 2000;

            //============================
            // Front     0
            // Middel  500
            // Back   1000
            //============================

            //======================================================================
            // FFF
            // Position 05 -> 500
            // IN  900
            // OUT 100
            m_F_L_Middle = 1000 - m_F_L_Middle;
            byte[] m_Data_Send_05 = Motor_Position_Set(5, m_F_L_Middle, m_F_L_Middle_Time);
            m_SerialPort_01.Write(m_Data_Send_05, 0, m_Data_Send_05.Length);
            Thread.Sleep(50);
            // Position 08 -> 500
            // IN  900
            // OUT 100
            m_F_R_Middle = 1000 - m_F_R_Middle;
            byte[] m_Data_Send_08 = Motor_Position_Set(8, m_F_R_Middle, m_F_R_Middle_Time);
            m_SerialPort_01.Write(m_Data_Send_08, 0, m_Data_Send_08.Length);
            Thread.Sleep(50);
            // Position 06 -> 500
            // IN  100
            // OUT 900
            byte[] m_Data_Send_06 = Motor_Position_Set(6, m_F_L_Gound, m_F_L_Gound_Time);
            m_SerialPort_01.Write(m_Data_Send_06, 0, m_Data_Send_06.Length);
            Thread.Sleep(1000);
            // Position 09 -> 500
            // IN  100
            // OUT 900
            byte[] m_Data_Send_09 = Motor_Position_Set(9, m_F_R_Gound, m_F_R_Gound_Time);
            m_SerialPort_01.Write(m_Data_Send_09, 0, m_Data_Send_09.Length);
            Thread.Sleep(50);
            //======================================================================
            // BBB
            // Position 02 -> 500
            // IN  100
            // OUT 900
            byte[] m_Data_Send_02 = Motor_Position_Set(2, m_B_L_Middle, m_B_L_Middle_Time);
            m_SerialPort_01.Write(m_Data_Send_02, 0, m_Data_Send_02.Length);
            Thread.Sleep(50);
            // Position 11 -> 500
            // IN  100
            // OUT 900
            byte[] m_Data_Send_11 = Motor_Position_Set(11, m_B_R_Middle, m_B_R_Middle_Time);
            m_SerialPort_01.Write(m_Data_Send_11, 0, m_Data_Send_11.Length);
            Thread.Sleep(50);
            // Position 03 -> 500
            // IN  900
            // OUT 100
            m_B_L_Gound = 1000 - m_B_L_Gound;
            byte[] m_Data_Send_03 = Motor_Position_Set(3, m_B_L_Gound, m_B_L_Gound_Time);
            m_SerialPort_01.Write(m_Data_Send_03, 0, m_Data_Send_03.Length);
            Thread.Sleep(50);
            // Position 12 -> 500
            // IN  900
            // OUT 100
            m_B_R_Gound = 1000 - m_B_R_Gound;
            byte[] m_Data_Send_12 = Motor_Position_Set(12, m_B_R_Gound, m_B_R_Gound_Time);
            m_SerialPort_01.Write(m_Data_Send_12, 0, m_Data_Send_12.Length);
            Thread.Sleep(50);
            //======================================================================
        }

        private void button21_Click(object sender, EventArgs e)
        {
            int m_F_L_Middle = 700;
            int m_F_R_Middle = 600;
            int m_F_L_Gound = 400;
            int m_F_R_Gound = 600;

            int m_B_L_Middle = 600;
            int m_B_R_Middle = 600;
            int m_B_L_Gound = 200;
            int m_B_R_Gound = 200;

            int m_F_L_Middle_Time = 2000;
            int m_F_R_Middle_Time = 2000;
            int m_F_L_Gound_Time = 2000;
            int m_F_R_Gound_Time = 2000;

            int m_B_L_Middle_Time = 2000;
            int m_B_R_Middle_Time = 2000;
            int m_B_L_Gound_Time = 2000;
            int m_B_R_Gound_Time = 2000;

            //============================
            // Front     0
            // Middel  500
            // Back   1000
            //============================

            //======================================================================
            // FFF
            // Position 05 -> 500
            // IN  900
            // OUT 100
            m_F_L_Middle = 1000 - m_F_L_Middle;
            byte[] m_Data_Send_05 = Motor_Position_Set(5, m_F_L_Middle, m_F_L_Middle_Time);
            m_SerialPort_01.Write(m_Data_Send_05, 0, m_Data_Send_05.Length);
            Thread.Sleep(50);
            // Position 08 -> 500
            // IN  900
            // OUT 100
            m_F_R_Middle = 1000 - m_F_R_Middle;
            byte[] m_Data_Send_08 = Motor_Position_Set(8, m_F_R_Middle, m_F_R_Middle_Time);
            m_SerialPort_01.Write(m_Data_Send_08, 0, m_Data_Send_08.Length);
            Thread.Sleep(50);
            // Position 06 -> 500
            // IN  100
            // OUT 900
            byte[] m_Data_Send_06 = Motor_Position_Set(6, m_F_L_Gound, m_F_L_Gound_Time);
            m_SerialPort_01.Write(m_Data_Send_06, 0, m_Data_Send_06.Length);
            Thread.Sleep(50);
            // Position 09 -> 500
            // IN  100
            // OUT 900
            byte[] m_Data_Send_09 = Motor_Position_Set(9, m_F_R_Gound, m_F_R_Gound_Time);
            m_SerialPort_01.Write(m_Data_Send_09, 0, m_Data_Send_09.Length);
            Thread.Sleep(50);
            //======================================================================
            // BBB
            // Position 02 -> 500
            // IN  100
            // OUT 900
            byte[] m_Data_Send_02 = Motor_Position_Set(2, m_B_L_Middle, m_B_L_Middle_Time);
            m_SerialPort_01.Write(m_Data_Send_02, 0, m_Data_Send_02.Length);
            Thread.Sleep(50);
            // Position 11 -> 500
            // IN  100
            // OUT 900
            byte[] m_Data_Send_11 = Motor_Position_Set(11, m_B_R_Middle, m_B_R_Middle_Time);
            m_SerialPort_01.Write(m_Data_Send_11, 0, m_Data_Send_11.Length);
            Thread.Sleep(50);
            // Position 03 -> 500
            // IN  900
            // OUT 100
            m_B_L_Gound = 1000 - m_B_L_Gound;
            byte[] m_Data_Send_03 = Motor_Position_Set(3, m_B_L_Gound, m_B_L_Gound_Time);
            m_SerialPort_01.Write(m_Data_Send_03, 0, m_Data_Send_03.Length);
            Thread.Sleep(50);
            // Position 12 -> 500
            // IN  900
            // OUT 100
            m_B_R_Gound = 1000 - m_B_R_Gound;
            byte[] m_Data_Send_12 = Motor_Position_Set(12, m_B_R_Gound, m_B_R_Gound_Time);
            m_SerialPort_01.Write(m_Data_Send_12, 0, m_Data_Send_12.Length);
            Thread.Sleep(50);
            //======================================================================
        }

        private void button41_Click(object sender, EventArgs e)
        {
            // Position Set 100
            byte[] m_Data_Send_01 = Motor_Position_Set(m_Motor_ID_01, 100, 0);
            m_SerialPort_01.Write(m_Data_Send_01, 0, m_Data_Send_01.Length);
        }

        private void button27_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        

        private void m_Text_Motor_ID_03_TextChanged(object sender, EventArgs e)
        {
          
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            button9_Click_1(null, null);
            Thread.Sleep(1);
            button6_Click(null, null);
            Thread.Sleep(10);
            button7_Click(null, null);
            if (m_Text_Motor_ID_01.Text == "8") { timer1.Enabled = false; button10.Text = "Ok"; button14_Click_1(null, null); }

        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            int changenumber = int.Parse(m_Text_Motor_ID_01.Text)+1;
            if (m_Text_Motor_ID_01.Text =="8")changenumber = 1;
            m_Text_Motor_ID_01.Text = changenumber.ToString();
    


        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            button10.Text = "Start";

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            button9_Click_1(null, null);
            Thread.Sleep(1);
            button4_Click(null, null);
            Thread.Sleep(5);
            if (m_Text_Motor_ID_01.Text == "8") { timer2.Enabled = false; button11.Text = "Enabled"; }
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            timer2.Enabled = true;
            button11.Text = "Enabling";
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            button9_Click_1(null, null);
            Thread.Sleep(1);
            button5_Click(null, null);
            Thread.Sleep(5);
            if (m_Text_Motor_ID_01.Text == "8") { timer3.Enabled = false; button12.Text = "Diabled"; }
        }

        private void button12_Click_1(object sender, EventArgs e)
        {
            timer3.Enabled = true;
            button12.Text = "Disabling";
        }

        private void button13_Click_1(object sender, EventArgs e)
        {
            timer4.Enabled = true;
            button13.Text = "Reseting";
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            button9_Click_1(null, null);
            Thread.Sleep(1);
            button2_Click(null, null);
            Thread.Sleep(1);
            if (m_Text_Motor_ID_01.Text == "8") { timer4.Enabled = false; button13.Text = "Reseted"; }
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void button14_Click_1(object sender, EventArgs e)
        {
            textBox9.Text=textBox1.Text+" " + textBox2.Text + " "+ textBox3.Text + " "+ textBox4.Text + " "+ textBox5.Text + " "+ textBox6.Text + " "+ textBox7.Text + " "+ textBox8.Text + " ";
        }

        private void button15_Click_1(object sender, EventArgs e)
        {
            f_Motor_Action(5, 668, 50, 100);                //Initial
            f_Motor_Action(6, 238, 50, 100);
            f_Motor_Action(1, 293, 50, 100);
            f_Motor_Action(2, 700, 50, 100);
            f_Motor_Action(3, 328, 50, 100);
            f_Motor_Action(4, 700, 50, 100);
            f_Motor_Action(7, 651, 50, 100);
            f_Motor_Action(8, 244, 50, 100);
        }

        private void button16_Click_1(object sender, EventArgs e)
        {       //Left

            f_Motor_Action(1, 293, 50, 100);
            f_Motor_Action(2, 700, 50, 100);
            f_Motor_Action(3, 328, 50, 100);
            f_Motor_Action(4, 700, 50, 100);
            f_Motor_Action(7, 650, 50, 100);
            f_Motor_Action(8, 150, 50, 100);
            Thread.Sleep(50);
            f_Motor_Action(5, 850, 50, 100);
            f_Motor_Action(6, 50, 50, 100);
            Thread.Sleep(50);
            f_Motor_Action(8, 344, 50, 100);
            f_Motor_Action(4, 600, 50, 100);
            f_Motor_Action(5, 800, 50, 100);
            f_Motor_Action(6, 350, 50, 100);
            Thread.Sleep(50);
            f_Motor_Action(5, 668, 50, 100);                //Initial
            f_Motor_Action(6, 238, 50, 100);
            f_Motor_Action(3, 328, 50, 100);
            f_Motor_Action(4, 700, 50, 100);
            f_Motor_Action(1, 293, 50, 100);
            f_Motor_Action(2, 700, 50, 100);
            f_Motor_Action(7, 651, 50, 100);
            f_Motor_Action(8, 244, 50, 100);
        }


        private void button17_Click_1(object sender, EventArgs e)
        {
            f_Motor_Action(1, 293, 50, 100);
            f_Motor_Action(2, 700, 50, 100);
            f_Motor_Action(3, 378, 50, 100);
            f_Motor_Action(4, 600, 50, 100);
            f_Motor_Action(7, 651, 50, 100);
            f_Motor_Action(8, 244, 50, 100);
            Thread.Sleep(50);
            f_Motor_Action(1, 50, 50, 100);
            f_Motor_Action(2, 850, 50, 100);
            Thread.Sleep(50);
            f_Motor_Action(8, 344, 50, 100);
            f_Motor_Action(4, 600, 50, 100);
            f_Motor_Action(1, 193, 50, 100);
            f_Motor_Action(2, 800, 50, 100);
            Thread.Sleep(50);
            f_Motor_Action(4, 700, 50, 100);
            f_Motor_Action(7, 651, 50, 100);
            f_Motor_Action(8, 244, 50, 100);
            f_Motor_Action(5, 668, 50, 100);                //Initial
            f_Motor_Action(6, 238, 50, 100);
            f_Motor_Action(1, 293, 50, 100);
            f_Motor_Action(2, 700, 50, 100);
            f_Motor_Action(3, 328, 50, 100);
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            if (!textBox10.Text.Equals("") && int.Parse(textBox10.Text) != 0)
            {
                byte[] m_Data_Send_01 = Motor_Position_Set(m_Motor_ID_01, int.Parse(textBox10.Text), 0);
                m_SerialPort_01.Write(m_Data_Send_01, 0, m_Data_Send_01.Length);

            }
        }

        private void m_MessageBox_01_TextChanged(object sender, EventArgs e)
        {

        }

        private void button18_Click_1(object sender, EventArgs e)
        {
            if(timer5.Enabled ==true) timer5.Enabled = false;
            else timer5.Enabled = true;
            button18.Text = "Running";
        }
        int count = 0;

        private void timer5_Tick(object sender, EventArgs e)
        {
            count++;
            switch (count)
            {
                case 1://front left go
                    f_Motor_Action(5, 750, 50, 100);
                    f_Motor_Action(7, 600, 50, 100);
                    f_Motor_Action(8, 400, 50, 100);
                    f_Motor_Action(6, 200, 50, 100);
                    break;
                case 2://front left reset
                    f_Motor_Action(5, 650, 50, 100);
                    f_Motor_Action(7, 650, 50, 100);
                    f_Motor_Action(8, 250, 50, 100);
                    f_Motor_Action(6, 240, 50, 100);
                    break;
                case 5:
                    f_Motor_Action(1, 210, 50, 100);//front right go
                    f_Motor_Action(3, 390, 50, 100);
                    f_Motor_Action(4, 550, 50, 100);
                    f_Motor_Action(2, 700, 50, 100);
                    break; 
                case 6:

                    f_Motor_Action(1, 310, 50, 100);//front right reset
                    f_Motor_Action(3, 340, 50, 100);
                    f_Motor_Action(4, 700, 50, 100);
                    f_Motor_Action(2, 740, 50, 100);
                    break;
                case 7: count = 0; break;

            }
        }

        /* backup
        private void timer5_Tick(object sender, EventArgs e)
        {
            count++;
            switch (count)
            {
                case 1://front left go
                    f_Motor_Action(5, 750, 50, 100);
                    f_Motor_Action(7, 603, 50, 100);
                    f_Motor_Action(8, 396, 50, 100);
                    f_Motor_Action(6, 209, 50, 100);
                    break;
                case 2://front left reset
                    f_Motor_Action(5, 668, 50, 100);
                    f_Motor_Action(7, 651, 50, 100);
                    f_Motor_Action(8, 244, 50, 100);
                    f_Motor_Action(6, 238, 50, 100);
                    break;
                case 5:
                    f_Motor_Action(1, 200, 50, 100);//front right go
                    f_Motor_Action(3, 373, 50, 100);
                    f_Motor_Action(4, 528, 50, 100);
                    f_Motor_Action(2, 712, 50, 100);
                    break;
                case 6:

                    f_Motor_Action(1, 298, 50, 100);//front right reset
                    f_Motor_Action(3, 328, 50, 100);
                    f_Motor_Action(4, 700, 50, 100);
                    f_Motor_Action(2, 740, 50, 100);
                    break;
                case 7: count = 0; break;

            }
        } */

        private void button19_Click_1(object sender, EventArgs e)
        {
            
                f_Motor_Action(5, 668, 50, 100);                //Initial
                f_Motor_Action(6, 138, 50, 100);
                f_Motor_Action(1, 293, 50, 100);
                f_Motor_Action(2, 800, 50, 100);
                f_Motor_Action(3, 500, 50, 100);
                f_Motor_Action(4, 500, 50, 100);
                f_Motor_Action(7, 500, 50, 100);
                f_Motor_Action(8, 500, 50, 100);
                 Thread.Sleep(50);
            f_Motor_Action(5, 500, 50, 100);
            f_Motor_Action(6, 508, 50, 100);
            f_Motor_Action(1, 503, 50, 100);
            f_Motor_Action(2, 500, 50, 100);
        }

        private void button20_Click_1(object sender, EventArgs e)
        {

        }
    }
}
