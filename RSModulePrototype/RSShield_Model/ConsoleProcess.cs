using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSShield_Model.ConsoleProcess
{
    public class ConsoleProcess
    {
        Process ServerProcess;

        private void initServerupdate()
        {
            PrintOnRichText("서버 업데이트중 입니다");
            updateProgressbar.Value = 0;
            updateProgressbar.Maximum = 100;

            CheckForIllegalCrossThreadCalls = false; //에러방지
            ServerProcess = new Process(); //변수에 Process 값 대입
            ServerProcess.StartInfo.FileName = "cmd.exe";
            //ServerProcess.StartInfo.WorkingDirectory = GetCutLink(SettingFilejsonObject["Link"].ToString(), 4);//cmd실행경로 수정
            ServerProcess.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;

            ServerProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden; //검은창 안뜨게
            ServerProcess.EnableRaisingEvents = true; //이벤트가 발생되도록
            ServerProcess.StartInfo.UseShellExecute = false;
            ServerProcess.StartInfo.RedirectStandardOutput = true; //서버 출력 메세지를 받아올지
            ServerProcess.StartInfo.RedirectStandardInput = true; //명령어를 입력할수 있도록 할지
            ServerProcess.StartInfo.CreateNoWindow = true; //창이 안뜨게
            ServerProcess.StartInfo.RedirectStandardError = true; //에러 출력 메세지를 받아올지

            ServerProcess.Exited += new EventHandler(ServerExit); //서버가 종료되었을때
            ServerProcess.OutputDataReceived += new DataReceivedEventHandler(ServerOut); //서버에서 출력메세지가 생겼을때
            ServerProcess.ErrorDataReceived += new DataReceivedEventHandler(ServerOut); //서버에서 오류메세지가 생겼을때

            ServerProcess.Start(); //서버 시작
            ServerProcess.BeginErrorReadLine(); //에러 메세지 가져오기 시작
            ServerProcess.BeginOutputReadLine(); //서버 출력 메세지 가져오기 시작
            
            ServerProcess.StandardInput.Write(@"cd steam
                                                    steamcmd.exe +@ShutdownOnFailedCommand 1
                                                    @NoPromptForPassword 1
                                                    login anonymous 
                                                    force_install_dir "+ GetCutLink(SettingFilejsonObject["Link"].ToString(), 3) + @"
                                                    app_update 258550 validate 
                                                    quit
                                                    cd ..
                                                    exit" + Environment.NewLine);
        }

        private void ServerExit(object sender, EventArgs e)
        {
            ServerProcess.Close();
            ServerProcess.Dispose();
            PrintOnRichText("ServerUpdate-업데이트 프로세스가 종료되었습니다.");
        }

        private void ServerOut(object sender, DataReceivedEventArgs e)
        {   
            try
            {
                consoleRichTextbox.AppendText("\n" + e.Data);

                if (e.Data.Substring(0, 41) == " Update state (0x5) validating, progress:")
                {
                    //PrintOnRichText(e.Data.Substring(42, 2));
                    if(e.Data.Substring(43, 1) == ".")
                    {
                        updateProgressbar.Value = Convert.ToInt16(e.Data.Substring(42, 1));
                    }
                    else
                    {
                        if (e.Data.Substring(44, 1) == ".")
                        {
                            updateProgressbar.Value = Convert.ToInt16(e.Data.Substring(42, 2));
                        }
                        else
                        {
                            updateProgressbar.Value = Convert.ToInt16(e.Data.Substring(42, 3));
                        }
                    }
                }

                if (e.Data.Substring(0, 43) == " Update state (0x61) downloading, progress:")
                {
                    //PrintOnRichText(e.Data.Substring(42, 2));
                    if (e.Data.Substring(45, 1) == ".")
                    {
                        updateProgressbar.Value = Convert.ToInt16(e.Data.Substring(44, 1));
                    }
                    else
                    {
                        if (e.Data.Substring(46, 1) == ".")
                        {
                            updateProgressbar.Value = Convert.ToInt16(e.Data.Substring(44, 2));
                        }
                        else
                        {
                            updateProgressbar.Value = Convert.ToInt16(e.Data.Substring(44, 3));
                        }
                    }
                }

                if (e.Data.Substring(0, 45) == " Update state (0x11) preallocating, progress:")
                {
                    //PrintOnRichText(e.Data.Substring(42, 2));
                    if (e.Data.Substring(47, 1) == ".")
                    {
                        updateProgressbar.Value = Convert.ToInt16(e.Data.Substring(46, 1));
                    }
                    else
                    {
                        if (e.Data.Substring(48, 1) == ".")
                        {
                            updateProgressbar.Value = Convert.ToInt16(e.Data.Substring(46, 2));
                        }
                        else
                        {
                            updateProgressbar.Value = Convert.ToInt16(e.Data.Substring(46, 3));
                        }
                    }
                }

            }
            catch { }
        }
    }
}
