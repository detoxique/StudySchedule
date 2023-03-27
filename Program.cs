using System;

namespace QuadroEngine
{
    class Program
    {
        public static Game game;

        [STAThread]
        static void Main(string[] args)
        {
            game = new Game("Study Schedule", 1280, 720);
            while (game.App.IsOpen)
            {
                if (game.OrganizationEntering != null)
                {
                    while (game.OrganizationEntering.IsOpen)
                    {
                        game.UpdateOrg();
                        if (game.RingsEditing != null)
                        {
                            while (game.RingsEditing.IsOpen)
                            {
                                game.UpdateRingsEditing();
                            }
                        }

                        if (game.WeekendsEditing != null)
                        {
                            while (game.WeekendsEditing.IsOpen)
                            {
                                game.UpdateWndsEntrForm();
                            }
                        }

                        if (game.TeachersEditing != null)
                        {
                            while (game.TeachersEditing.IsOpen)
                            {
                                game.UpdateTchrsEdtng();
                                if (game.TeacherSchedule != null)
                                {
                                    game.UpdateTeacherSchedule();
                                }
                            }
                        }

                        if (game.ClassesAdding != null)
                        {
                            while (game.ClassesAdding.IsOpen)
                            {
                                game.UpdateClassesForm();
                            }
                        }
                    }
                }
                if (game.AppInfo != null)
                {
                    while (game.AppInfo.IsOpen)
                    {
                        game.AppInfoUpdate();
                    }
                }
                game.Update();
            }
        }
    }
}
