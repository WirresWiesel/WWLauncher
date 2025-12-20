// Outdated and not in use anymore
namespace Launcher
{
    class ProgramSample
    {
        public static List<Programinfo> GetExamples()
        {
            Programinfo program1 = new Programinfo
            {
                Name = "OpenKneeboard",
                State = "not Running",
                EXEPath = @"C:\Program Files\OpenKneeboard\OpenKneeboard.exe"
            };

            Programinfo program2 = new Programinfo
            {
                Name = "SRS",
                State = "not Running",
                EXEPath = @"C:\Program Files\SRS\SRS.exe"
            };

            return new List<Programinfo> {
                program1,
                program2
            };
        }
    }
}