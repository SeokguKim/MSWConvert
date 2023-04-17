using System.IO;
using System.Numerics;
using System.Text;

namespace app54
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Type the path to .mod or .txt file...");
            string inpath = Console.In.ReadLine();
            if (!File.Exists(inpath) || Path.GetExtension(inpath) != ".mod" && Path.GetExtension(inpath) != ".txt")
            {
                Console.WriteLine("Invaild Path or Extension... Quitting Program...");
                return;
            }
            //경로 체크
            bool ismod = Path.GetExtension(inpath) == ".mod";
            string outpath = Path.GetDirectoryName(inpath) + "\\" + Path.GetFileNameWithoutExtension(inpath);
            outpath += ismod ? ".txt" : ".mod";
            if (File.Exists(outpath)) File.Delete(outpath);
            //확장자에 따라 출력 파일 경로 설정
            if (ismod) mod2txt(inpath, outpath);
            else txt2mod(inpath, outpath);
            //해당하는 함수 실행
            Console.WriteLine("Conversion Complete!");
            //The end
        }

        static void mod2txt(string modpath, string outpath)
        {
            //.mod to .txt 함수
            Stream fs = new FileStream(modpath, FileMode.Open);
            byte[] wholebytes = new byte[fs.Length];
            fs.Read(wholebytes, 0, wholebytes.Length);
            //새 파일스트림으로 읽음
            List<byte> li = new List<byte>();
            Stack<byte> st = new Stack<byte>();
            //문자열 처리할 리스트와 스택
            for (int i = 0; i < wholebytes.Length; i++)
            {
                Console.Clear();
                Console.WriteLine($"Processing .mod to .txt... {i + 1} / {wholebytes.Length}");
                //진행 상황 표시
                if (st.Count == 0)
                {
                    //빈 스택일때의 처리
                    if (wholebytes[i] < 128)
                    {
                        //아스키로 표현가능
                        if (wholebytes[i] == 123) st.Push(123);
                        //여는 중괄호가 나오면 스택에 삽입
                        li.Add(wholebytes[i]);
                        //리스트에 더함
                    }
                    else
                    {
                        //아스키로 표현불가
                        li.Add(124);
                        li.Add((byte)(wholebytes[i] - 128));
                        //해당 바이트는 아스키 범위를 넘어섬을 가리키는 문자 삽입 후 -128 해서 아스키 범위에 집어넣고 더함
                    }
                }
                else
                {
                    //스택이 비어있지 않을 경우
                    byte sttop = st.Peek();
                    //스택 위를 본다
                    if (sttop == 34 && wholebytes[i] == 34) st.Pop();
                    //따옴표가 스택에 있고 따옴표면 스택 위를 날림
                    else if (wholebytes[i] == 34) st.Push(34);
                    //그냥 따옴표면 스택에 삽입
                    else if (sttop == 123 && wholebytes[i] == 125) st.Pop();
                    //스택 위가 여는 중괄호고 닫는 중괄호가 오면 스택 위를 날림
                    else if (sttop != 34 && wholebytes[i] == 123) st.Push(123);
                    //여는 중괄호고 따옴표가 없으면 스택에 삽입
                    li.Add(wholebytes[i]);
                    //리스트에 더함
                }
            }

            byte[] res = li.ToArray();
            string resstr = Encoding.UTF8.GetString(res);
            File.WriteAllText(outpath, resstr);
            //리스트 > 배열 > 문자열로 변환해 .txt 파일에 쓰기
        }

        static void txt2mod(string txtpath, string outpath)
        {
            //.txt to .mod 함수 (이 프로그램으로 처리한 것만 제대로 됨)
            Stream fs = new FileStream(txtpath, FileMode.Open);
            byte[] wholebytes = new byte[fs.Length];
            fs.Read(wholebytes, 0, wholebytes.Length);
            //새 파일스트림으로 읽음
            List<byte> li = new List<byte>();
            Stack<byte> st = new Stack<byte>();
            //문자열 처리할 리스트와 스택
            for (int i = 0; i < wholebytes.Length; i++)
            {
                Console.Clear();
                Console.WriteLine($"Processing .txt to .mod... {i + 1} / {wholebytes.Length}");
                //진행 상황 표시
                if (st.Count == 0)
                {
                    //빈 스택일때의 처리
                    if (wholebytes[i] == 124)
                    {
                        //유니코드로 표현 불가능한 문자 지시 기호
                        li.Add((byte)(wholebytes[i + 1] + 128));
                        i++;
                        //다음 문자를 복원해 리스트에 더하고 오프셋
                    }
                    else
                    {
                        //일반
                        if (wholebytes[i] == 123) st.Push(123);
                        //여는 중괄호면 스택에 삽입
                        li.Add(wholebytes[i]);
                        //리스트에 더함
                    }
                }
                else
                {
                    byte sttop = st.Peek();
                    //스택 위를 본다
                    if (sttop == 34 && wholebytes[i] == 34) st.Pop();
                    //따옴표가 스택에 있고 따옴표면 스택 위를 날림
                    else if (wholebytes[i] == 34) st.Push(34);
                    //그냥 따옴표면 스택에 삽입
                    else if (sttop == 123 && wholebytes[i] == 125) st.Pop();
                    //스책의 위가 여는 중괄호고 닫는 중괄호가 오면 스택의 위를 날림
                    else if (sttop != 34 && wholebytes[i] == 123) st.Push(123);
                    //여는 중괄호고 따옴표가 없으면 스택에 삽입
                    li.Add(wholebytes[i]);
                    //리스트에 더함
                }
            }

            byte[] res = li.ToArray();
            File.WriteAllBytes(outpath, res);
            //리스트 > 배열로 변환해 .mod 파일에 바이트로 쓰기
        }
    }
}