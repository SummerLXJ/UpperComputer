using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpperComputer
{
    class Error_Rate_Statistics
    {
        Method method = new Method();
        private int Begin_Pos = 0; //这是开始搜索帧头的位置
        public int begin_pos
        {
            get { return Begin_Pos; }
            set { Begin_Pos = value;}
        }


        private byte[] Frame = null;
        public byte[] frame   //遥控或者遥测帧的信息,这种样子的定义应该先赋值才能给其他元素赋值
        {
            get { return Frame; }
            set { Frame = value; }
        }

        private int Losing_Lock_Num = 0; //帧失锁数
        public int losing_lock_num
        {
            get { return Losing_Lock_Num; }
            set { Losing_Lock_Num = value; }
        }

        private byte Sum_Verify = 0;
        public byte sum_verify
        {
            get { return Sum_Verify; }
            set { Sum_Verify = value; }
        }
        public byte verify()   //遥控或遥测帧的和校验
        {
            sum_verify = method.sum_verify(frame);
            return sum_verify;
        }

        //private int First=0;
        //public int first   //是否是第一帧
        //{
        //    get { return First; }
        //    set { First = value; }
        //}

        private string Single_str = null;
        public string single_str  //下一帧数据
        {
            get { return Single_str; }
            set { Single_str = value; }
        }

        private string Last_Str = null;
        public string last_str   // 上一帧数据
        {
            get { return Last_Str; }
            set { Last_Str = value; }
        }
        private string Frame_head = null; // 帧头对应的长度
        public string frame_head
        {
            get { return Frame_head; }
            set { Frame_head = value; }
        }

        private bool HeadIsFind = false;
        public bool headisfind //确定是否已经开始解调
        {
            get { return HeadIsFind; }
            set { HeadIsFind = value; }
        }

        private bool DecodedIsset = false;
        public bool decodedIsset //确定是否已经开始解调
        {
            get { return DecodedIsset; }
            set { DecodedIsset = value; }
        }
        private string Combine_str=null;
        public string combine_str //前后连接组合帧
        {
            get { return Combine_str; }
            set { Combine_str = value; }
        }

        private int Receive_Sum = 0;  //接收到的bit数
        public int receive_sum
        {
            get { return Receive_Sum; }
            set { Receive_Sum = value; }
        }

        private UInt16 Frame_Length = 0; //帧长
        public UInt16 frame_length
        {
            get { return Frame_Length; }
            set { Frame_Length = value; }
        }
        private int Error_Code_num = 0;
        public int error_code_sum    //误码总数
        {
            get { return Error_Code_num; }
            set { Error_Code_num = value; }
        }
        private string Base_String = null;
        public string base_string
        {
            get { return Base_String; }
            set { Base_String = value; }
        }
        private string Decode_Frame = null;
        public string decode_frame   //这是依据设置解调后获得的解调帧
        {
            get { return Decode_Frame; }
            set { Decode_Frame = value; }
        }

        private int Count = 0;
        public int count //count是收到的统计开始后接收到的遥测帧计数，count=receive_sum/8
        {
            get { return Count; }
            set { Count = value; }
        }
        private double Error_Code_Rate = 0;
        public double error_code_rate
        {
            get { return Error_Code_Rate; }
            set { Error_Code_Rate = value; }
        }

        private bool Statistics_Begin = false;
        public bool statistics_begin
        {
            get { return Statistics_Begin; }
            set { Statistics_Begin = value; }
        }
        public void statistics()
        {
            receive_sum = (receive_sum + frame_length * 8);
            error_code_sum = error_code_sum + method.error_code_com(decode_frame, base_string);
            count = count + 1;
            if (receive_sum == 0)
            { error_code_rate = 0; }
            else
            { error_code_rate = (double)error_code_sum / (receive_sum); }
        }

       
    }
}
