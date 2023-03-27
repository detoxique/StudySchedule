using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

using QuadroEngine.UI;

using KeyEventArgs = SFML.Window.KeyEventArgs;
using View = SFML.Graphics.View;
using System.Xml.Linq;
using TextBox = QuadroEngine.UI.TextBox;
using Button = QuadroEngine.UI.Button;
using System.Linq.Expressions;
using CheckBox = QuadroEngine.UI.CheckBox;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using System.Security.Cryptography.X509Certificates;
using System.Collections;
using DocumentFormat.OpenXml.Spreadsheet;
using Text = SFML.Graphics.Text;
using Font = SFML.Graphics.Font;
using Color = SFML.Graphics.Color;
using Panel = QuadroEngine.UI.Panel;
using Page = QuadroEngine.UI.Page;

namespace QuadroEngine
{
    public enum Theme
    {
        Dark,
        Light
    };
    public class Teacher
    {
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Cabinet { get; set; }

        public bool[,] TimeTable { get; set; }

        public Teacher(string name, string subject, string cabinet)
        {
            Name = name;
            Subject = subject;
            Cabinet = cabinet;
            TimeTable = new bool[7, 10];
            for (int i = 0; i < 7; i++)
                for (int j = 0; j < 10; j++)
                {
                    TimeTable[i, j] = true;
                }
        }
        public Teacher(string name, string subject, string cabinet, bool[,] timeTable)
        {
            Name = name;
            Subject = subject;
            Cabinet = cabinet;
            TimeTable = timeTable;
        }
    }
    public class Subject
    {
        public string SubjectName;
        public Teacher TeacerOfSubject;
        public int Hours = 0;
        public Subject(string subjectname, Teacher teacher, int hrs)
        {
            SubjectName = subjectname;
            TeacerOfSubject = teacher;
            Hours = hrs;
        }
    }
    public class StudyingClass
    {
        public string Grade;
        public string Letter;
        public List<Subject> Subjects;
        public Subject[,] TimeTable = new Subject[7,10];
        public StudyingClass(string grade, string letter, List<Subject> subjects)
        {
            Grade = grade;
            Letter = letter;
            Subjects = subjects;
        }
    }
    public class Game
    {
        // main app
        public RenderWindow App;
        public Button Create;
        public Button Print;
        public Text welcome;
        public Layer MainLayer;
        public Layer Notifocations;
        public List<TabWithElements> tabs;
        public List<Text> TabLabels;
        public TopBar MainTopBar;
        public Animator animator;
        public Animator animator2;
        private bool Opened = false;

        // organization entering
        public RenderWindow OrganizationEntering;
        private Clock DTime2 = new Clock();
        public float DeltaTime2 = 0;

        // Weekends editing
        public RenderWindow WeekendsEditing;
        private Clock Dtime3 = new Clock();
        public float DeltaTime3 = 0;

        // Rings editing
        public RenderWindow RingsEditing;
        private Clock Dtime4 = new Clock();
        public float DeltaTime4 = 0;

        // Teachers editing
        public RenderWindow TeachersEditing;
        private Clock Dtime5 = new Clock();
        public float DeltaTime5 = 0;

        // Single Teacher Schedule editing
        public RenderWindow TeacherSchedule;
        private Clock Dtime6 = new Clock();
        public float DeltaTime6 = 0;

        // Classes adding
        public RenderWindow ClassesAdding;
        private Clock Dtime7 = new Clock();
        public float DeltaTime7 = 0;

        // Program info
        public RenderWindow AppInfo;
        private Clock Dtime8 = new Clock();
        public float DeltaTime8 = 0;

        // Saving
        public RenderWindow SavingApp;
        private Clock Dtime9 = new Clock();
        public float DeltaTime9 = 0;

        // main logic
        public string OrganizationName = "", StudyingYear = "";
        public int LessonsPerDay = 7, StudyingDays = 5;
        public List<string> lessonsRings;
        public List<DayOfTheWeek> StudyingDaysList;
        public List<Teacher> Teachers;
        public List<StudyingClass> StudyingClassesList;

        private Clock DTime = new Clock();
        public float DeltaTime = 0, FPS = 0, MaxFPS = 0, Width = 0, Height = 0;
        public Entity Camera = new Entity();
        public GameObject character;
        public Slider sl;
        public Text Debug;

        public List<Font> Fonts;
        public List<UI_Element> UIs;
        public List<Layer> Layers;
        public List<GameObject> gameObjects;

        private Notification LoadingError;
        private Notification AreUSure;

        public Color Background = new Color(255, 255, 255);
        public bool FPSDraw = true;
        public Theme theme = Theme.Dark;

        public TabWithElements testTab;
        public List<Element> elements;

        private OpenFileDialog ofd;
        private OpenFileDialog ofdCSV;

        public Game(string title, uint width, uint height)
        {
            ContextSettings settings = new ContextSettings(1, 0, 4);
            
            App = new RenderWindow(new VideoMode(width, height), title, Styles.Default, settings);
            App.SetFramerateLimit(60);

            Width = width;
            Height = height;
            //App.SetIcon()

            Initialize();
            ofd = new OpenFileDialog();
            ofd.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";

            ofdCSV = new OpenFileDialog();
            ofdCSV.Filter = "CSV Files (*.csv)|*.csv";

            Image icon = new Image("5.png");
            App.SetIcon(512, 512, icon.Pixels);
        }

        public Game(string title)
        {
            ContextSettings settings = new ContextSettings(1, 0, 1);
            App = new RenderWindow(new VideoMode(VideoMode.DesktopMode.Width, VideoMode.DesktopMode.Height), title, Styles.Fullscreen, settings);

            Initialize();
        }

        public void Update()
        {
            DeltaTime = DTime.ElapsedTime.AsSeconds();
            DTime.Restart();

            App.DispatchEvents();
            App.Clear(Background);

            Input();

            try
            {
                foreach (UI_Element ui in UIs)
                {
                    ui.Update(DeltaTime);
                }
            } 
            catch { }

            if (!Opened)
            {
                // 380 300
                welcome.Position = new Vector2f(380, animator.Lerp(welcome.Position.Y, 300, DeltaTime));
                // 548 368
                Create.Position = new Vector2f(548, animator2.Lerp(Create.Position.Y, 368, DeltaTime));

                if (welcome.Position == new Vector2f(380, 300) && Create.Position == new Vector2f(548, 368))
                    Opened = true;
            }

            try
            {
                foreach (Layer lr in Layers)
                {
                    App.Draw(lr);
                }
            }
            catch { }

            App.Display();
            FPS++;
        }

        private void Input()
        {
            
        }

        private void Initialize()
        {
            Fonts = new List<Font>();
            Layers = new List<Layer>();
            UIs = new List<UI_Element>();
            gameObjects = new List<GameObject>();
            
            StudyingDaysList = new List<DayOfTheWeek>();
            Teachers = new List<Teacher>();
            StudyingClassesList = new List<StudyingClass>();

            Fonts.Add(new Font("font.ttf"));
            Fonts.Add(new Font("Inter-Regular.ttf"));

            animator = new Animator();
            animator.To = 0.35f;

            animator2 = new Animator();
            animator2.To = 0.45f;

            MainLayer = new Layer();
            Notifocations = new Layer();

            Create = new Button(new Vector2f(548, 668), new Text("Составить расписание", Fonts[0], 14), new Vector2f(185, 45));
            Create.action = EnterOrganization;

            UIs.Add(Create);
            MainLayer.Objects.Add(Create);

            welcome = new Text("Добро пожаловать в Study Schedule!", Fonts[0], 28);
            welcome.Position = new Vector2f(380, 600);
            welcome.FillColor = Color.Black;
            MainLayer.Objects.Add(welcome);

            List<ColumnRowInfo> columns = new List<ColumnRowInfo>();
            List<ColumnRowInfo> rows = new List<ColumnRowInfo>();
            elements = new List<Element>();

            LoadingError = new Notification("Не удалось открыть файл.", "ОK", Fonts[0], new Vector2f(1280, 720));
            LoadingError.CurrentFont = Fonts[0];
            LoadingError.Closed = true;
            UIs.Add(LoadingError);
            Notifocations.Objects.Add(LoadingError);

            AreUSure = new Notification("У вас есть несохраненные изменения.\n\nВы действительно хотите выйти?", "Да", "Нет", App.Close, null, Fonts[0], new Vector2f(1280, 720));
            AreUSure.CurrentFont = Fonts[0];
            AreUSure.Closed = true;
            UIs.Add(AreUSure);
            Notifocations.Objects.Add(AreUSure);

            Layers.Add(MainLayer);
            Layers.Add(Notifocations);

            //foreach (UI_Element ui in UIs)
            //{
            //    ui.Update(0);
            //}

            App.Closed += App_Closed;
            App.KeyPressed += App_KeyPressed;
            App.MouseButtonPressed += App_MouseButtonPressed;
            App.MouseButtonReleased += App_MouseButtonReleased;
            App.MouseMoved += App_MouseMoved;
            App.MouseWheelScrolled += App_MouseWheelScrolled;
            App.TextEntered += App_TextEntered;
            App.Resized += App_Resized;
            App.KeyReleased += App_KeyReleased;
        }

        private void LoadListFromFile()
        {
            if (ofd.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = ofd.FileName;

            try
            {
                for (int i = 0; i < File.ReadAllLines(filename).Length; i++)
                {
                    elements.Add(new Element(new Vector2f(125, 95 + 45 * i), new Text(File.ReadAllLines(filename)[i], new Font("font.ttf"), 14), new Vector2f(75, 45)));
                }
            }
            catch
            {
                LoadingError.Closed = false;
            }

            Debug.DisplayedString = elements.Count.ToString();
        }

        public List<UI_Element> UIs2 = new List<UI_Element>();
        public List<Layer> Layers2 = new List<Layer>();

        public void EnterOrganization()
        {
            ContextSettings settings = new ContextSettings(1, 0, 1);
            OrganizationEntering = new RenderWindow(new VideoMode(550, 450), "Enter Organization Name", Styles.Default, settings);
            OrganizationEntering.SetFramerateLimit(60);

            Image icon = new Image("5.png");
            OrganizationEntering.SetIcon(512, 512, icon.Pixels);

            InitializeOrgEntForm();
        }

        public void UpdateOrg()
        {
            DeltaTime2 = DTime2.ElapsedTime.AsSeconds();
            DTime2.Restart();

            OrganizationEntering.DispatchEvents();
            App.DispatchEvents();
            OrganizationEntering.Clear(Background);

            foreach (UI_Element ui in UIs2)
            {
                ui.Update(DeltaTime2);
            }

            aplyOrg.Position = OrgButtonAnimator.Lerp(aplyOrg.Position, new Vector2f(410, 386), DeltaTime2);

            foreach (Layer lr in Layers2)
            {
                OrganizationEntering.Draw(lr);
            }

            OrganizationEntering.Display();
        }

        private TextBox orgnameTextBox;
        private TextBox syear;
        private TextBox daysperweek;
        private TextBox subsperday;

        private Text dayspwerror;
        private Text lessonspderror;

        private Button aplyOrg;
        private Animator OrgButtonAnimator;
        private void InitializeOrgEntForm()
        {
            Layers2 = new List<Layer>();
            UIs2 = new List<UI_Element>();

            Layer l = new Layer();

            OrgButtonAnimator = new Animator();
            OrgButtonAnimator.To = 0.5f;

            aplyOrg = new Button
            {
                Position = new Vector2f(410, 386),
                Label = new Text("Применить", Fonts[0], 14),
                Size = new Vector2f(100, 40),
                action = ApplyOrgNameForm
            };
            UIs2.Add(aplyOrg);
            l.Objects.Add(UIs2[0]);

            orgnameTextBox = new TextBox(new Vector2f(50, 42), new Vector2f(200, 25));
            UIs2.Add(orgnameTextBox);
            l.Objects.Add(orgnameTextBox);

            Text title = new Text("Название организации:", Fonts[0], 14);
            title.Position = new Vector2f(50, 20);
            title.FillColor = Color.Black;
            l.Objects.Add(title);

            Text enterYear = new Text("Учебный год:", Fonts[0], 14);
            enterYear.Position = new Vector2f(50, 80);
            enterYear.FillColor = Color.Black;
            l.Objects.Add(enterYear);

            syear = new TextBox(new Vector2f(50, 102), new Vector2f(200, 25));
            syear.text = "2022/2023";
            UIs2.Add(syear);
            l.Objects.Add(syear);

            Text daysperweektext = new Text("Учебных дней:", Fonts[0], 14);
            daysperweektext.Position = new Vector2f(50, 150);
            daysperweektext.FillColor = Color.Black;
            l.Objects.Add(daysperweektext);

            daysperweek = new TextBox(new Vector2f(50, 172), new Vector2f(50, 25));
            daysperweek.text = "5";
            UIs2.Add(daysperweek);
            l.Objects.Add(daysperweek);

            Text subsperdaytext = new Text("Уроков в день:", Fonts[0], 14);
            subsperdaytext.Position = new Vector2f(50, 204);
            subsperdaytext.FillColor = Color.Black;
            l.Objects.Add(subsperdaytext);

            subsperday = new TextBox(new Vector2f(50, 226), new Vector2f(50, 25));
            subsperday.text = "8";
            UIs2.Add(subsperday);
            l.Objects.Add(subsperday);

            Button EditRings = new Button(new Vector2f(170, 214), new Text("Ред. время звонков", Fonts[0], 14), new Vector2f(158, 42));
            EditRings.action = OpenRingsEditingForm;
            UIs2.Add(EditRings);
            l.Objects.Add(EditRings);

            Button EditWeekends = new Button(new Vector2f(170, 156), new Text("Ред. выходные", Fonts[0], 14), new Vector2f(158, 42));
            EditWeekends.action = EnteringWeekends;
            UIs2.Add(EditWeekends);
            l.Objects.Add(EditWeekends);

            dayspwerror = new Text("Неверный формат", Fonts[0], 14);
            dayspwerror.Position = new Vector2f(345, 172);
            dayspwerror.FillColor = Color.White;
            l.Objects.Add(dayspwerror);

            lessonspderror = new Text("Неверный формат", Fonts[0], 14);
            lessonspderror.Position = new Vector2f(345, 226);
            lessonspderror.FillColor = Color.White;
            l.Objects.Add(lessonspderror);

            Text teacherandsubs = new Text("Добавить преподавателей, кабинеты и предметы", Fonts[0], 14);
            teacherandsubs.Position = new Vector2f(50, 276);
            teacherandsubs.FillColor = Color.Black;
            l.Objects.Add(teacherandsubs);

            Button addteachersAndSubs = new Button(new Vector2f(50, 300), new Text("Добавить/Загрузить", Fonts[0], 14), new Vector2f(164, 38));
            addteachersAndSubs.action = EditingTeachers;
            UIs2.Add(addteachersAndSubs);
            l.Objects.Add(addteachersAndSubs);

            Text classes = new Text("Добавить классы и изучаемые ими предметы", Fonts[0], 14);
            classes.Position = new Vector2f(50, 362);
            classes.FillColor = Color.Black;
            l.Objects.Add(classes);

            Button addclasses = new Button(new Vector2f(50, 386), new Text("Добавить/Загрузить", Fonts[0], 14), new Vector2f(164, 38));
            addclasses.action = AddingClasses;
            UIs2.Add(addclasses);
            l.Objects.Add(addclasses);

            Layers2.Add(l);

            OrganizationEntering.Closed += OrganizationEntering_Closed;
            OrganizationEntering.KeyPressed += OrganizationEntering_KeyPressed;
            OrganizationEntering.MouseButtonPressed += OrganizationEntering_MouseButtonPressed;
            OrganizationEntering.MouseButtonReleased += OrganizationEntering_MouseButtonReleased;
            OrganizationEntering.MouseMoved += OrganizationEntering_MouseMoved;
            OrganizationEntering.MouseWheelScrolled += OrganizationEntering_MouseWheelScrolled;
            OrganizationEntering.TextEntered += OrganizationEntering_TextEntered;
            OrganizationEntering.Resized += OrganizationEntering_Resized;
            OrganizationEntering.KeyReleased += OrganizationEntering_KeyReleased;
        }

        public void ApplyOrgNameForm()
        {
            OrganizationName = orgnameTextBox.text;
            StudyingYear = syear.text;

            bool stdaysDone = false;
            try
            {
                StudyingDays = Convert.ToInt16(daysperweek.text);
                dayspwerror.FillColor = Color.White;
                stdaysDone = true;
            }
            catch
            {
                dayspwerror.FillColor = Color.Black;
            }

            bool lessonsPDayDone = false;
            try
            {
                LessonsPerDay = Convert.ToInt16(subsperday.text);
                lessonspderror.FillColor = Color.White;
                lessonsPDayDone = true;
            }
            catch
            {
                lessonspderror.FillColor = Color.Black;
            }

            if (lessonsPDayDone && stdaysDone)
            {
                UIs.Remove(Create);
                MainLayer.Objects.Remove(Create);
                MainLayer.Objects.Remove(welcome);

                CreateTimetable();

                OrganizationEntering.Close();
            }
        }

        public void CreateTimetable()///////////////////////////////////////////// Составление расписания
        {
            TabLabels = new List<Text>();
            tabs = new List<TabWithElements>();
            Panel panel = new Panel(new Vector2f(20, 30), new Vector2f(1240, 465), Fonts[0]);

            Page page1 = new Page();
            Page page2 = new Page();
            Page page3 = new Page();
            Page page4 = new Page();
            Page page5 = new Page();
            Page page6 = new Page();

            #region Creating
            Random rnd = new Random();
            for (int i = 0; i < StudyingClassesList.Count; i++)
            {
                for (int l = 0; l < LessonsPerDay; l++)
                {
                    for (int d = 0; d < StudyingDays; d++)
                    {
                        int index = rnd.Next(0, StudyingClassesList[i].Subjects.Count - 1);
                        int checkedlessons = 0;

                        bool isntBusy = true;

                        while (checkedlessons < StudyingClassesList[i].Subjects.Count)
                        {
                            if (StudyingClassesList[i].Subjects[index].Hours > 0 && StudyingClassesList[i].Subjects[index].TeacerOfSubject.TimeTable[d, l]) // Проверка на кол-во часов и возможность преподавателя вести урок
                            {

                                for (int j = 0; j < StudyingClassesList.Count; j++)
                                {
                                    checkedlessons++;
                                    if (StudyingClassesList[j].TimeTable[d, l] != null)
                                    {
                                        if (StudyingClassesList[j].TimeTable[d, l].SubjectName == StudyingClassesList[i].Subjects[index].SubjectName)
                                        {
                                            isntBusy = false;
                                            index = rnd.Next(0, StudyingClassesList[i].Subjects.Count - 1);
                                            break;
                                        }
                                    }
                                }

                                if (isntBusy)
                                {
                                    StudyingClassesList[i].TimeTable[d, l] = StudyingClassesList[i].Subjects[index];
                                    StudyingClassesList[i].Subjects[index].Hours--;
                                    checkedlessons = StudyingClassesList[i].Subjects.Count;
                                }
                            }
                            else
                            {
                                checkedlessons++;
                                index = rnd.Next(0, StudyingClassesList[i].Subjects.Count - 1);
                            }
                        }
                    }
                }
            }
            #endregion

            for (int i = 0; i < StudyingClassesList.Count; i++)
            {
                List<ColumnRowInfo> TableColumns = new List<ColumnRowInfo>();
                List<ColumnRowInfo> TableRows = new List<ColumnRowInfo>();

                List<Element> els = new List<Element>();

                for (int d = 0; d < StudyingDays; d++)
                {
                    for (int l = 0; l < LessonsPerDay; l++)
                    {
                        try
                        {
                            if (StudyingClassesList[i].TimeTable[d, l] != null)
                            {
                                string shortsubname = StudyingClassesList[i].TimeTable[d, l].SubjectName.Remove(7) + ".";
                                Element el;
                                if (i % 2 == 0)
                                    el = new Element(new Vector2f(125 + d * 75, 95 + l * 45), new Text(shortsubname, Fonts[0], 14), new Vector2f(75, 45));
                                else
                                    el = new Element(new Vector2f(125 + (StudyingDays) * 75 + d * 75 + 120, 95 + l * 45), new Text(shortsubname, Fonts[0], 14), new Vector2f(75, 45));
                                el.SubLabel = new Text(StudyingClassesList[i].TimeTable[d, l].TeacerOfSubject.Cabinet, Fonts[0], 8);
                                els.Add(el);
                            }
                        }
                        catch { }
                    }
                }

                foreach (DayOfTheWeek dw in StudyingDaysList)
                {
                    switch (dw)
                    {
                        case DayOfTheWeek.Monday:
                            TableColumns.Add(new ColumnRowInfo("пн", new Vector2f(75, 45)));
                            break;
                        case DayOfTheWeek.Tuesday:
                            TableColumns.Add(new ColumnRowInfo("вт", new Vector2f(75, 45)));
                            break;
                        case DayOfTheWeek.Wednesday:
                            TableColumns.Add(new ColumnRowInfo("ср", new Vector2f(75, 45)));
                            break;
                        case DayOfTheWeek.Thursday:
                            TableColumns.Add(new ColumnRowInfo("чт", new Vector2f(75, 45)));
                            break;
                        case DayOfTheWeek.Friday:
                            TableColumns.Add(new ColumnRowInfo("пт", new Vector2f(75, 45)));
                            break;
                        case DayOfTheWeek.Saturday:
                            TableColumns.Add(new ColumnRowInfo("сб", new Vector2f(75, 45)));
                            break;
                        case DayOfTheWeek.Sunday:
                            TableColumns.Add(new ColumnRowInfo("вс", new Vector2f(75, 45)));
                            break;
                    }
                }

                for (int j = 1; j <= LessonsPerDay; j++)
                    TableRows.Add(new ColumnRowInfo(j.ToString() + " урок", new Vector2f(75, 45)));

                Text label = new Text(StudyingClassesList[i].Grade + " " + StudyingClassesList[i].Letter, Fonts[0], 14);
                //Text label = new Text(TableColumns.Count + " " + TableRows.Count, Fonts[0], 14);
                label.FillColor = Color.Black;
                
                
                TabLabels.Add(label);
                //MainLayer.Objects.Add(label);

                TabWithElements table;

                if (i % 2 == 0)
                {
                    table = new TabWithElements(TableColumns, TableRows, new Vector2f(50, 50), els);
                    label.Position = new Vector2f(87 - label.GetGlobalBounds().Width / 2, 72 - label.GetGlobalBounds().Height / 1.5f);

                }
                else
                {
                    table = new TabWithElements(TableColumns, TableRows, new Vector2f(50 + StudyingDays * 75 + 120, 50), els);
                    label.Position = new Vector2f(87 + StudyingDays * 75 + 120 - label.GetGlobalBounds().Width / 2, 72 - label.GetGlobalBounds().Height / 1.5f);
                }

                //TabWithElements table = new TabWithElements(TableColumns, TableRows, new Vector2f(50 + i * StudyingDays * 75 + i * 120, 50), els);
                tabs.Add(table);
                
                switch (i)
                {
                    case 0:
                        page1.BasicElements.Add(label);
                        page1.Elements.Add(table);
                        panel.Pages.Add(page1);
                        break;
                    case 1:
                        page1.BasicElements.Add(label);
                        page1.Elements.Add(table);
                        break;
                    case 2:
                        page2.BasicElements.Add(label);
                        page2.Elements.Add(table);
                        panel.Pages.Add(page2);
                        break;
                    case 3:
                        page2.BasicElements.Add(label);
                        page2.Elements.Add(table);
                        break;
                    case 4:
                        page3.BasicElements.Add(label);
                        page3.Elements.Add(table);
                        panel.Pages.Add(page3);
                        break;
                    case 5:
                        page3.BasicElements.Add(label);
                        page3.Elements.Add(table);
                        break;
                    case 6:
                        page4.BasicElements.Add(label);
                        page4.Elements.Add(table);
                        panel.Pages.Add(page4);
                        break;
                    case 7:
                        page4.BasicElements.Add(label);
                        page4.Elements.Add(table);
                        break;
                    case 8:
                        page5.BasicElements.Add(label);
                        page5.Elements.Add(table);
                        panel.Pages.Add(page5);
                        break;
                    case 9:
                        page5.BasicElements.Add(label);
                        page5.Elements.Add(table);
                        break;
                    case 10:
                        page6.BasicElements.Add(label);
                        page6.Elements.Add(table);
                        panel.Pages.Add(page6);
                        break;
                }

                //UIs.Add(table);
                //MainLayer.Objects.Add(table);
            }

            MainTopBar = new TopBar(1280, 25);

            List<Button> buttons = new List<Button>();
            buttons.Add(new Button(new Vector2f(0, 0), new Text("Cоздать", Fonts[0], 14), new Vector2f(85, 25), EnterOrganization));
            buttons.Add(new Button(new Vector2f(0, 0), new Text("Cохранить", Fonts[0], 14), new Vector2f(85, 25), Save));
            Print = new Button(new Vector2f(0, 0), new Text("Печать", Fonts[0], 14), new Vector2f(82, 25));
            //Print.Clickable = false;
            buttons.Add(Print);
            buttons.Add(new Button(new Vector2f(0, 25), new Text("Закрыть", Fonts[0], 14), new Vector2f(82, 25), SureToClose));
            MainTopBar.AddItem(new Text("файл", Fonts[0], 14), 85, buttons);

            List<Button> buttons2 = new List<Button>();
            //buttons2.Add(new Button(new Vector2f(0, 0), new Text("ахуй", Fonts[0], 14), new Vector2f(50, 25)));
            //buttons2.Add(new Button(new Vector2f(0, 25), new Text("фига", Fonts[0], 14), new Vector2f(50, 25)));
            MainTopBar.AddItem(new Text("Правка", Fonts[0], 14), 60, buttons2);

            List<Button> buttons3 = new List<Button>();
            buttons3.Add(new Button(new Vector2f(0, 0), new Text("О программе", Fonts[0], 14), new Vector2f(50, 25), AppInfoInitialize));
            MainTopBar.AddItem(new Text("Помощь", Fonts[0], 14), 102, buttons3);
            //MainTopBar.AddItem(new Text("Помощь", Fonts[0], 14), 68, buttons);

            UIs.Add(panel);
            MainLayer.Objects.Add(panel);

            UIs.Add(MainTopBar);
            MainLayer.Objects.Add(MainTopBar);
        }

        public void SureToClose()
        {
            AreUSure.Closed = false;
        }

        public void Save()
        {
            //Texture screen = new Texture((uint)App.GetView().Viewport.Width, (uint)App.GetView().Viewport.Height);
            //screen.Update(App);
            //if (screen.CopyToImage().SaveToFile("raspipanie.png"))
            //{

            //}
            foreach (StudyingClass sc in StudyingClassesList)
            {
                //Texture raspisanie = new Texture();
                
            }
            Image image = new Image(1280, 720);
            
            Texture screen = new Texture((uint)App.GetView().Viewport.Width, (uint)App.GetView().Viewport.Height);
            screen.Update(App);

            image = screen.CopyToImage();
            if (image.SaveToFile("raspisanie.png"))
            {

            }
        }

        public void OpenRingsEditingForm()
        {
            try
            {
                LessonsPerDay = Convert.ToInt16(subsperday.text);
                lessonspderror.FillColor = Color.White;

                ContextSettings settings = new ContextSettings(1, 0, 4);
                RingsEditing = new RenderWindow(new VideoMode(650, 550), "Edit Rings Schedule", Styles.Default, settings);
                RingsEditing.SetFramerateLimit(60);

                Image icon = new Image("5.png");
                RingsEditing.SetIcon(512, 512, icon.Pixels);

                InitializeRingsEditingForm();
            }
            catch
            {
                lessonspderror.FillColor = Color.Black;
            }
        }

        public List<UI_Element> UIs4;
        public List<Layer> Layers4;
        public TabWithElements Rings;
        public List<TextBox> Breaks;

        private TextBox firstlessontext;
        private TextBox lessondurationtext;

        public List<Text> RingsDisplay;

        public void InitializeRingsEditingForm()
        {
            UIs4 = new List<UI_Element>();
            Layers4 = new List<Layer>();
            Layer l = new Layer();
            Breaks = new List<TextBox>();
            RingsDisplay = new List<Text>();

            UIs4.Add(new Button
            {
                Position = new Vector2f(510, 490),
                Label = new Text("Применить", Fonts[0], 14),
                Size = new Vector2f(100, 40),
                action = RingsEditing.Close
            });
            l.Objects.Add(UIs4[0]);

            List<ColumnRowInfo> rows = new List<ColumnRowInfo>();
            List<ColumnRowInfo> cols = new List<ColumnRowInfo>();
            cols.Add(new ColumnRowInfo("Время", new Vector2f(75, 45)));

            for (int i = 1; i <= Convert.ToInt16(subsperday.text); i++)
            {
                rows.Add(new ColumnRowInfo(i + " урок", new Vector2f(75, 45)));
                
                if (i + 1 <= Convert.ToInt16(subsperday.text))
                {
                    CircleShape circ = new CircleShape(3);
                    circ.Position = new Vector2f(230, 169 + i * 30);
                    circ.FillColor = Color.Black;
                    l.Objects.Add(circ);

                    Text text = new Text("Перемена между " + i + " и " + (i + 1) + " уроком:", Fonts[0], 14);
                    text.Position = new Vector2f(245, 163 + i * 30);
                    text.FillColor = Color.Black;
                    l.Objects.Add(text);

                    TextBox textbox = new TextBox(new Vector2f(485, 160 + i * 30), new Vector2f(50, 25));
                    Breaks.Add(textbox);
                    UIs4.Add(textbox);
                    l.Objects.Add(textbox);
                }
            }

            Rings = new TabWithElements(cols, rows, new Vector2f(50, 50), null);
            l.Objects.Add(Rings);
            UIs4.Add(Rings);

            Text firstlesson = new Text("Время начала первого урока:", Fonts[0], 14);
            firstlesson.Position = new Vector2f(230, 50);
            firstlesson.FillColor = Color.Black;
            l.Objects.Add(firstlesson);

            firstlessontext = new TextBox(new Vector2f(230, 73), new Vector2f(130, 25));
            firstlessontext.text = "8:30";
            UIs4.Add(firstlessontext);
            l.Objects.Add(firstlessontext);

            Text lessonduration = new Text("Продолжительность урока(в минутах):", Fonts[0], 14);
            lessonduration.Position = new Vector2f(230, 103);
            lessonduration.FillColor = Color.Black;
            l.Objects.Add(lessonduration);

            lessondurationtext = new TextBox(new Vector2f(230, 126), new Vector2f(130, 25));
            lessondurationtext.text = "40";
            UIs4.Add(lessondurationtext);
            l.Objects.Add(lessondurationtext);

            Text breakstext = new Text("Продолжительности перемен(в минутах):", Fonts[0], 14);
            breakstext.Position = new Vector2f(230, 157);
            breakstext.FillColor = Color.Black;
            l.Objects.Add(breakstext);

            Button applyBtn = new Button(new Vector2f(510, 430), new Text("Составить", Fonts[0], 14), new Vector2f(100, 40));
            applyBtn.action = RingsButton;
            UIs4.Add(applyBtn);
            l.Objects.Add(applyBtn);

            Layers4.Add(l);

            RingsEditing.Closed += RingsEditing_Closed;
            RingsEditing.KeyPressed += RingsEditing_KeyPressed;
            RingsEditing.MouseButtonPressed += RingsEditing_MouseButtonPressed;
            RingsEditing.MouseButtonReleased += RingsEditing_MouseButtonReleased;
            RingsEditing.MouseMoved += RingsEditing_MouseMoved;
            RingsEditing.MouseWheelScrolled += RingsEditing_MouseWheelScrolled;
            RingsEditing.TextEntered += RingsEditing_TextEntered;
            RingsEditing.Resized += RingsEditing_Resized;
            RingsEditing.KeyReleased += RingsEditing_KeyReleased;
        }

        public void UpdateRingsEditing()
        {
            DeltaTime4 = DTime.ElapsedTime.AsSeconds();
            Dtime4.Restart();

            RingsEditing.DispatchEvents();
            App.DispatchEvents();
            RingsEditing.Clear(Background);

            foreach (UI_Element ui in UIs4)
            {
                ui.Update(DeltaTime4);
            }

            foreach (Layer lr in Layers4)
            {
                RingsEditing.Draw(lr);
            }

            RingsEditing.Display();
        }

        public void RingsButton() ////////////////////// TODO ДОПИСАТЬ
        {
            string fls = "";
            fls = firstlessontext.text;

            string lesdur = lessondurationtext.text;

            int time = 0, duration = 0;

            try
            {
                time += Convert.ToInt32(fls.Split(':')[0]) * 60 + Convert.ToInt32(fls.Split(':')[1]);

                duration = Convert.ToInt32(lesdur);

                int firstles = (time + duration);
                //string firstLesString = (firstles % 60) + " -\n" + firstles - (firstles % 60);

                //essonsRings.Add(firstLesString);

                foreach (TextBox tb in Breaks)
                {

                }
            }
            catch
            {

            }
        }

        public void EnteringWeekends()
        {
            try
            {
                StudyingDays = Convert.ToInt32(daysperweek.text);
                if (StudyingDays > 7)
                {
                    dayspwerror.DisplayedString = "Нельзя сделать более 7 учебных дней";
                    dayspwerror.FillColor = Color.Black;
                }
                else
                {
                    dayspwerror.FillColor = Color.White;

                    ContextSettings settings = new ContextSettings(1, 0, 4);
                    WeekendsEditing = new RenderWindow(new VideoMode(650, 350), "Edit Weekends Schedule", Styles.Default, settings);
                    WeekendsEditing.SetFramerateLimit(60);

                    Image icon = new Image("5.png");
                    WeekendsEditing.SetIcon(512, 512, icon.Pixels);

                    InitializeWndsEntrForm();
                }
            }
            catch { }
        }

        public List<UI_Element> UIs3;
        public List<Layer> Layers3;
        public TabWithElements WeekendsTab;
        public List<CheckBox> Checkboxes;

        public void InitializeWndsEntrForm()
        {
            UIs3 = new List<UI_Element>();
            Layers3 = new List<Layer>();
            Checkboxes = new List<CheckBox>();
            Layer l = new Layer();

            List<ColumnRowInfo> rows = new List<ColumnRowInfo>();
            List<ColumnRowInfo> cols = new List<ColumnRowInfo>();

            rows.Add(new ColumnRowInfo("Выходные", new Vector2f(90, 45)));
            cols.Add(new ColumnRowInfo("Пн", new Vector2f(60, 45)));
            cols.Add(new ColumnRowInfo("Вт", new Vector2f(60, 45)));
            cols.Add(new ColumnRowInfo("Ср", new Vector2f(60, 45)));
            cols.Add(new ColumnRowInfo("Чт", new Vector2f(60, 45)));
            cols.Add(new ColumnRowInfo("Пт", new Vector2f(60, 45)));
            cols.Add(new ColumnRowInfo("Сб", new Vector2f(60, 45)));
            cols.Add(new ColumnRowInfo("Вс", new Vector2f(60, 45)));

            WeekendsTab = new TabWithElements(cols, rows, new Vector2f(50, 50), null);

            UIs3.Add(WeekendsTab);
            l.Objects.Add(WeekendsTab);

            Button apply = new Button(new Vector2f(510, 270), new Text("Применить", Fonts[0], 14), new Vector2f(100, 40));
            apply.action = ApplyWeekends;
            UIs3.Add(apply);
            l.Objects.Add(apply);

            for (int i = 0; i < 7; i++)
            {
                CheckBox cb = new CheckBox();
                cb.Position = new Vector2f(162 + i * 60, 110);
                cb.Checked = true;
                Checkboxes.Add(cb);
                UIs3.Add(cb);
                l.Objects.Add(cb);
            }

            for (int i = 0; i < StudyingDays; i++)
            {
                Checkboxes[i].Checked = false;
            }

            Text otmet = new Text("Отметьте выходные дни:", Fonts[0], 14);
            otmet.Position = new Vector2f(50, 20);
            otmet.FillColor = Color.Black;
            l.Objects.Add(otmet);

            Layers3.Add(l);

            WeekendsEditing.Closed += WeekendsEditing_Closed;
            WeekendsEditing.KeyPressed += WeekendsEditing_KeyPressed;
            WeekendsEditing.MouseButtonPressed += WeekendsEditing_MouseButtonPressed;
            WeekendsEditing.MouseButtonReleased += WeekendsEditing_MouseButtonReleased;
            WeekendsEditing.MouseMoved += WeekendsEditing_MouseMoved;
            WeekendsEditing.MouseWheelScrolled += WeekendsEditing_MouseWheelScrolled;
            WeekendsEditing.TextEntered += WeekendsEditing_TextEntered;
            WeekendsEditing.Resized += WeekendsEditing_Resized;
            WeekendsEditing.KeyReleased += WeekendsEditing_KeyReleased;
        }

        public void ApplyWeekends()
        {
            foreach (CheckBox cb in Checkboxes)
            {
                switch (Checkboxes.IndexOf(cb))
                {
                    case 0:
                        if (!cb.Checked)
                            StudyingDaysList.Add(DayOfTheWeek.Monday);
                        break;
                    case 1:
                        if (!cb.Checked)
                            StudyingDaysList.Add(DayOfTheWeek.Tuesday);
                        break;
                    case 2:
                        if (!cb.Checked)
                            StudyingDaysList.Add(DayOfTheWeek.Wednesday);
                        break;
                    case 3:
                        if (!cb.Checked)
                            StudyingDaysList.Add(DayOfTheWeek.Thursday);
                        break;
                    case 4:
                        if (!cb.Checked)
                            StudyingDaysList.Add(DayOfTheWeek.Friday);
                        break;
                    case 5:
                        if (!cb.Checked)
                            StudyingDaysList.Add(DayOfTheWeek.Saturday);
                        break;
                    case 6:
                        if (!cb.Checked)
                            StudyingDaysList.Add(DayOfTheWeek.Sunday);
                        break;
                }
            }

            WeekendsEditing.Close();
        }

        public void UpdateWndsEntrForm()
        {
            DeltaTime3 = Dtime3.ElapsedTime.AsSeconds();
            Dtime3.Restart();

            App.DispatchEvents();
            WeekendsEditing.DispatchEvents();
            WeekendsEditing.Clear(Background);

            foreach (UI_Element ui in UIs3)
            {
                ui.Update(DeltaTime3);
            }

            foreach (Layer lr in Layers3)
            {
                WeekendsEditing.Draw(lr);
            }

            WeekendsEditing.Display();
        }

        public void EditingTeachers()
        {
            if (StudyingDaysList.Count == 0)
            {
                try
                {
                    StudyingDays = Convert.ToInt32(daysperweek.text);

                    for (int i = 0; i < StudyingDays; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                StudyingDaysList.Add(DayOfTheWeek.Monday);
                                break;
                            case 1:
                                StudyingDaysList.Add(DayOfTheWeek.Tuesday);
                                break;
                            case 2:
                                StudyingDaysList.Add(DayOfTheWeek.Wednesday);
                                break;
                            case 3:
                                StudyingDaysList.Add(DayOfTheWeek.Thursday);
                                break;
                            case 4:
                                StudyingDaysList.Add(DayOfTheWeek.Friday);
                                break;
                            case 5:
                                StudyingDaysList.Add(DayOfTheWeek.Saturday);
                                break;
                            case 6:
                                StudyingDaysList.Add(DayOfTheWeek.Sunday);
                                break;
                        }
                    }
                }
                catch { }
            }

            ContextSettings settings = new ContextSettings(1, 0, 4);
            TeachersEditing = new RenderWindow(new VideoMode(1080, 750), "Edit Teachers List", Styles.Default, settings);
            TeachersEditing.SetFramerateLimit(60);

            Image icon = new Image("5.png");
            TeachersEditing.SetIcon(512, 512, icon.Pixels);

            InitializeTeachersEditingForm();
        }

        public List<UI_Element> UIs5;
        public List<Layer> Layers5;

        public List<ColumnRowInfo> cols;
        public List<ColumnRowInfo> rows;
        public List<Element> Subjects;

        public TabWithElements TeachersTab;
        public TextBox TeacherInitials;
        public TextBox TeacherSubject;
        public TextBox TeacherAudience;
        private Layer l2;

        private ColumnRowInfo prepodRow;

        public int TeachersCount = 0;

        public void InitializeTeachersEditingForm()
        {
            UIs5 = new List<UI_Element>();
            Layers5 = new List<Layer>();
            l2 = new Layer();

            #region Tab

            ColumnRowInfo subjectsColumn = new ColumnRowInfo("Предмет", new Vector2f(180, 20));
            ColumnRowInfo audiencenum = new ColumnRowInfo("№ Кабинета", new Vector2f(100, 20));

            cols = new List<ColumnRowInfo>();
            rows = new List<ColumnRowInfo>();
            Subjects = new List<Element>();

            cols.Add(subjectsColumn);
            cols.Add(audiencenum);

            if (Teachers.Count == 0)
            {
                prepodRow = new ColumnRowInfo("имя преподавателя", new Vector2f(270, 20));
                rows.Add(prepodRow);

                TeachersTab = new TabWithElements(cols, rows, new Vector2f(50, 50), Subjects);
                UIs5.Add(TeachersTab);
                l2.Objects.Add(TeachersTab);
            }
            else
            {
                foreach (Teacher t in Teachers)
                {
                    prepodRow = new ColumnRowInfo(t.Name, new Vector2f(270, 20));
                    rows.Add(prepodRow);

                    Element sub = new Element(new Vector2f(320, 70 + (rows.Count - 1) * 20), new Text(t.Subject, Fonts[0], 14), new Vector2f(180, 20));
                    sub.Dragable = false;
                    Subjects.Add(sub);

                    Element cab = new Element(new Vector2f(500, 70 + (rows.Count - 1) * 20), new Text(t.Cabinet, Fonts[0], 14), new Vector2f(100, 20));
                    cab.Dragable = false;
                    Subjects.Add((cab));
                }

                TeachersTab = new TabWithElements(cols, rows, new Vector2f(50, 50), Subjects);
                UIs5.Add(TeachersTab);
                l2.Objects.Add(TeachersTab);
            }

            #endregion

            Button apply = new Button(new Vector2f(940, 660), new Text("Применить", Fonts[0], 14), new Vector2f(100, 40));
            apply.action = TeachersEditing.Close;
            UIs5.Add(apply);
            l2.Objects.Add(apply);

            #region Add Teacher

            //Sprite dropShadow = new Sprite(new Texture(@"resources\tchraddingdropshadow.png"));
            //dropShadow.Position = new Vector2f(420, 20);
            //l.Objects.Add(dropShadow);

            Text addtchrtext = new Text("Добавить преподавателя", Fonts[0], 14);
            addtchrtext.Position = new Vector2f(780, 50);
            addtchrtext.FillColor = Color.Black;
            l2.Objects.Add(addtchrtext);

            Text enterinittext = new Text("Введите ФИО преподавателя:", Fonts[0], 14);
            enterinittext.Position = new Vector2f(710, 80);
            enterinittext.FillColor = Color.Black;
            l2.Objects.Add(enterinittext);

            TeacherInitials = new TextBox(new Vector2f(710, 100), new Vector2f(275, 25));
            UIs5.Add(TeacherInitials);
            l2.Objects.Add(TeacherInitials);

            Text entersubtext = new Text("Введите преподаваемый предмет:", Fonts[0], 14);
            entersubtext.Position = new Vector2f(710, 135);
            entersubtext.FillColor = Color.Black;
            l2.Objects.Add(entersubtext);

            TeacherSubject = new TextBox(new Vector2f(710, 160), new Vector2f(200, 25));
            UIs5.Add(TeacherSubject);
            l2.Objects.Add(TeacherSubject);

            Text aoao = new Text("Введите номер кабинета:", Fonts[0], 14);
            aoao.Position = new Vector2f(710, 195);
            aoao.FillColor = Color.Black;
            l2.Objects.Add(aoao);

            TeacherAudience = new TextBox(new Vector2f(710, 220), new Vector2f(100, 25));
            UIs5.Add(TeacherAudience);
            l2.Objects.Add(TeacherAudience);

            Button applySubject = new Button(new Vector2f(920, 220), new Text("Добавить", Fonts[0], 14), new Vector2f(100, 40));
            applySubject.action = AddTeacher;
            UIs5.Add(applySubject);
            l2.Objects.Add(applySubject);

            Button loadTeachersFormCSVButton = new Button(new Vector2f(920, 270), new Text("Загрузить из .CSV", Fonts[0], 14), new Vector2f(140, 40));
            loadTeachersFormCSVButton.action = LoadTeachersFromCSV;
            UIs5.Add(loadTeachersFormCSVButton);
            l2.Objects.Add(loadTeachersFormCSVButton);

            #endregion

            Layers5.Add(l2);

            TeachersEditing.Closed += TeachersEditing_Closed;
            TeachersEditing.KeyPressed += TeachersEditing_KeyPressed;
            TeachersEditing.MouseButtonPressed += TeachersEditing_MouseButtonPressed;
            TeachersEditing.MouseButtonReleased += TeachersEditing_MouseButtonReleased;
            TeachersEditing.MouseMoved += TeachersEditing_MouseMoved;
            TeachersEditing.MouseWheelScrolled += TeachersEditing_MouseWheelScrolled;
            TeachersEditing.TextEntered += TeachersEditing_TextEntered;
            TeachersEditing.Resized += TeachersEditing_Resized;
            TeachersEditing.KeyReleased += TeachersEditing_KeyReleased;
        }

        public void UpdateTchrsEdtng()
        {
            DeltaTime5 = Dtime5.ElapsedTime.AsSeconds();
            Dtime5.Restart();

            App.DispatchEvents();
            TeachersEditing.DispatchEvents();
            TeachersEditing.Clear(Background);

            try
            {
                foreach (UI_Element ui in UIs5)
                {
                    ui.Update(DeltaTime5);
                }
                foreach (Layer l in Layers5)
                {
                    TeachersEditing.Draw(l);
                }
            }
            catch { }

            TeachersEditing.Display();
        }

        private bool firsttchradded = false;

        public void AddTeacher()
        {
            UIs5.Remove(TeachersTab);
            l2.Objects.Remove(TeachersTab);

            prepodRow = new ColumnRowInfo(TeacherInitials.text, new Vector2f(270, 20));

            if (firsttchradded)
            {
                rows.Add(prepodRow);
            }
            else
            {
                rows[0] = prepodRow;
                firsttchradded = true;
            }

            Element sub = new Element(new Vector2f(320, 70 + (rows.Count - 1) * 20), new Text(TeacherSubject.text, Fonts[0], 14), new Vector2f(180, 20));
            sub.Dragable = false; 
            Subjects.Add(sub);

            Element cab = new Element(new Vector2f(500, 70 + (rows.Count - 1) * 20), new Text(TeacherAudience.text, Fonts[0], 14), new Vector2f(100, 20));
            cab.Dragable = false;
            Subjects.Add((cab));

            //Element edit = new Element(new Vector2f(500, 95 + (rows.Count - 1) * 45), new Text("Редактировать", Fonts[0], 14), new Vector2f(125, 45));
            //edit.Dragable = false;
            //edit.action = EditingTeacherSchedule;
            //Subjects.Add(edit);

            Teachers.Add(new Teacher(TeacherInitials.text, TeacherSubject.text, TeacherAudience.text));

            TeachersTab = new TabWithElements(cols, rows, new Vector2f(50, 50), Subjects);

            UIs5.Add(TeachersTab);
            l2.Objects.Add(TeachersTab);

            EditingTeacherSchedule();
        }

        public async void LoadTeachersFromCSV() ///////////////////////////////////////////////
        {
            UIs5.Remove(TeachersTab);
            l2.Objects.Remove(TeachersTab);

            cols = new List<ColumnRowInfo>();
            rows = new List<ColumnRowInfo>();
            Subjects = new List<Element>();

            ColumnRowInfo subjectsColumn = new ColumnRowInfo("Предмет", new Vector2f(180, 20));
            ColumnRowInfo audiencenum = new ColumnRowInfo("№ Кабинета", new Vector2f(100, 20));

            cols.Add(subjectsColumn);
            cols.Add(audiencenum);

            string allfile = "";
            string[] cells;

            /////////// Загрузка из файла
            if (ofdCSV.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            string filename = ofdCSV.FileName;

            using (StreamReader streamReader = new StreamReader(filename))
            {
                
                allfile = await streamReader.ReadToEndAsync();
                allfile = allfile.Replace('\u000D', ' ');
                allfile = allfile.Replace('\u000A', ';');
                cells = allfile.Split(';');
            }

            List<Teacher> teachersfromcsv = new List<Teacher>();
            for (int i = 0; i < cells.Length / 3; i++)
            {
                teachersfromcsv.Add(new Teacher(cells[i * 3], cells[i * 3 + 1], cells[i * 3 + 2]));
                //teachersfromcsv.Add(new Teacher(cells.Length.ToString(), "хуй", "пизда"));
            }

            foreach (Teacher t in teachersfromcsv)
            {
                Teachers.Add(t);

                prepodRow = new ColumnRowInfo(t.Name, new Vector2f(270, 20));
                rows.Add(prepodRow);

                Element subj = new Element(new Vector2f(320, 70 + (rows.Count - 1) * 20), new Text(t.Subject, Fonts[0], 14), new Vector2f(180, 20));
                subj.Dragable = false;
                Subjects.Add(subj);

                Element cab = new Element(new Vector2f(500, 70 + (rows.Count - 1) * 20), new Text(t.Cabinet, Fonts[0], 14), new Vector2f(100, 20));
                cab.Dragable = false;
                Subjects.Add((cab));
            }

            TeachersTab = new TabWithElements(cols, rows, new Vector2f(50, 50), Subjects);

            UIs5.Add(TeachersTab);
            l2.Objects.Add(TeachersTab);
        }

        private string nametchr = "";

        public void EditingTeacherSchedule()
        {
            nametchr = TeacherInitials.text;

            TeacherInitials.text = "";
            TeacherSubject.text = "";
            TeacherAudience.text = "";

            ContextSettings settings = new ContextSettings(1, 0, 4);
            TeacherSchedule = new RenderWindow(new VideoMode(650, 550), "Edit Teacher Schedule", Styles.Default, settings);
            TeacherSchedule.SetFramerateLimit(60);

            Image icon = new Image("5.png");
            TeacherSchedule.SetIcon(512, 512, icon.Pixels);

            InitializeTeacherScheduleForm();
        }

        public List<UI_Element> UIs6;
        public List<Layer> Layers6;

        public TabWithElements TeacherActiveLessons;
        public CheckBox[,] ActiveLesson = new CheckBox[7, 10];

        public void InitializeTeacherScheduleForm()
        {
            UIs6 = new List<UI_Element>();
            Layers6 = new List<Layer>();
            Layer layerrr = new Layer();

            #region Tab
            List<ColumnRowInfo> cols = new List<ColumnRowInfo>();
            List<ColumnRowInfo> rows = new List<ColumnRowInfo>();

            for (int i = 0; i < StudyingDaysList.Count; i++)
            {
                switch (StudyingDaysList[i])
                {
                    case DayOfTheWeek.Monday:
                        cols.Add(new ColumnRowInfo("Пн", new Vector2f(60, 45)));
                        break;
                    case DayOfTheWeek.Tuesday:
                        cols.Add(new ColumnRowInfo("Вт", new Vector2f(60, 45)));
                        break;
                    case DayOfTheWeek.Wednesday:
                        cols.Add(new ColumnRowInfo("Ср", new Vector2f(60, 45)));
                        break;
                    case DayOfTheWeek.Thursday:
                        cols.Add(new ColumnRowInfo("Чт", new Vector2f(60, 45)));
                        break;
                    case DayOfTheWeek.Friday:
                        cols.Add(new ColumnRowInfo("Пт", new Vector2f(60, 45)));
                        break;
                    case DayOfTheWeek.Saturday:
                        cols.Add(new ColumnRowInfo("Сб", new Vector2f(60, 45)));
                        break;
                    case DayOfTheWeek.Sunday:
                        cols.Add(new ColumnRowInfo("Вс", new Vector2f(60, 45)));
                        break;
                }
            }

            for (int i = 1; i <= LessonsPerDay; i++)
            {
                rows.Add(new ColumnRowInfo(i.ToString(), new Vector2f(60, 45)));
            }

            TeacherActiveLessons = new TabWithElements(cols, rows, new Vector2f(50, 50), null);
            UIs6.Add(TeacherActiveLessons);
            layerrr.Objects.Add(TeacherActiveLessons);
            #endregion

            Text tchrsch = new Text("Расписание преподавателя: " + nametchr, Fonts[0], 14);
            tchrsch.Position = new Vector2f(35, 10);
            tchrsch.FillColor = Color.Black;
            layerrr.Objects.Add(tchrsch);

            for (int i = 0; i < StudyingDays; i++)
            {
                for (int j = 0; j < LessonsPerDay; j++)
                {
                    CheckBox cb = new CheckBox();
                    cb.Position = new Vector2f(130 + i * 60, 110 + j * 45);
                    cb.Checked = true;
                    ActiveLesson[i, j] = cb;
                    UIs6.Add(cb);
                    layerrr.Objects.Add(cb);
                }
            }

            Text info = new Text("*Если в определенный день недели нельзя ставить урок\nданного преподавателя, уберите отметку.", Fonts[0], 14);
            info.Position = new Vector2f(50, 460);
            info.FillColor = Color.Black;
            layerrr.Objects.Add(info);

            Button apply = new Button(new Vector2f(510, 460), new Text("Применить", Fonts[0], 14), new Vector2f(100, 40));
            apply.action = TeacherSchedule.Close;
            UIs6.Add(apply);
            layerrr.Objects.Add(apply);

            Layers6.Add(layerrr);

            TeacherSchedule.Closed += TeacherSchedule_Closed;
            TeacherSchedule.KeyPressed += TeacherSchedule_KeyPressed;
            TeacherSchedule.MouseButtonPressed += TeacherSchedule_MouseButtonPressed;
            TeacherSchedule.MouseButtonReleased += TeacherSchedule_MouseButtonReleased;
            TeacherSchedule.MouseMoved += TeacherSchedule_MouseMoved;
            TeacherSchedule.MouseWheelScrolled += TeacherSchedule_MouseWheelScrolled;
            TeacherSchedule.TextEntered += TeacherSchedule_TextEntered;
            TeacherSchedule.Resized += TeacherSchedule_Resized;
            TeacherSchedule.KeyReleased += TeacherSchedule_KeyReleased;
        }

        public void ApplyTeacherSchedule()
        {
            bool[,] timetable = new bool[7,10];

            for (int i = 0; i < StudyingDays; i++)
            {
                for (int j = 0; j < LessonsPerDay; j++)
                {
                    timetable[i, j] = ActiveLesson[i, j].Checked;
                }
            }

            Teachers.Add(new Teacher(TeacherInitials.text, TeacherSubject.text, TeacherAudience.text, timetable));
        }

        public void UpdateTeacherSchedule()
        {
            DeltaTime6 = Dtime6.ElapsedTime.AsSeconds();
            Dtime6.Restart();

            App.DispatchEvents();
            TeacherSchedule.DispatchEvents();
            TeacherSchedule.Clear(Background);

            try
            {
                foreach (UI_Element ui in UIs6)
                {
                    ui.Update(DeltaTime6);
                }
                foreach (Layer l in Layers6)
                {
                    TeacherSchedule.Draw(l);
                }
            }
            catch { }

            TeacherSchedule.Display();
        }

        public void AddingClasses()
        {
            ContextSettings settings = new ContextSettings(1, 0, 4);
            ClassesAdding = new RenderWindow(new VideoMode(1280, 750), "Add Classes", Styles.Default, settings);
            ClassesAdding.SetFramerateLimit(60);

            Image icon = new Image("5.png");
            ClassesAdding.SetIcon(512, 512, icon.Pixels);

            InitializeClassesAddingForm();
        }

        public List<UI_Element> UIs7;
        public List<Layer> Layers7;

        public TabWithElements ClassesTab;
        public List<ColumnRowInfo> ClassesTabColumns;
        public List<ColumnRowInfo> ClassesTabRows;
        public List<Element> ClassesTabElements;

        public TextBox GradeTextBox;
        public TextBox LetterTextBox;
        public TextBox ClassTeacher;

        public List<CheckBox> SubsCheckBoxes;
        public List<TextBox> SubsTextBoxes;
        public Layer layerclasses;

        public void InitializeClassesAddingForm()
        {
            UIs7 = new List<UI_Element>();
            Layers7 = new List<Layer>();
            layerclasses = new Layer();

            Text label = new Text("Добавить класс", Fonts[0], 14);
            label.FillColor = Color.Black;
            label.Position = new Vector2f(790, 25);
            layerclasses.Objects.Add(label);

            #region Tab
            ClassesTabColumns = new List<ColumnRowInfo>();
            ClassesTabRows = new List<ColumnRowInfo>();
            ClassesTabElements = new List<Element>();

            ClassesTabColumns.Add(new ColumnRowInfo("Классный руководитель", new Vector2f(270, 20)));
            ClassesTabColumns.Add(new ColumnRowInfo("Изучаемые предметы", new Vector2f(270, 20)));
            ClassesTabRows.Add(new ColumnRowInfo("", new Vector2f(52, 20)));

            ClassesTab = new TabWithElements(ClassesTabColumns, ClassesTabRows, new Vector2f(50, 50), ClassesTabElements);
            UIs7.Add(ClassesTab);
            layerclasses.Objects.Add(ClassesTab);

            Text lblontab = new Text("Класс", Fonts[0], 14);
            lblontab.FillColor = Color.Black;
            lblontab.Position = new Vector2f(53, 50);
            layerclasses.Objects.Add(lblontab);
            UIs7.Add(ClassesTab);
            #endregion

            Text label2 = new Text("Класс: ", Fonts[0], 14);
            label2.FillColor = Color.Black;
            label2.Position = new Vector2f(690, 70);
            layerclasses.Objects.Add(label2);

            GradeTextBox = new TextBox(new Vector2f(750, 67), new Vector2f(40, 25));
            layerclasses.Objects.Add(GradeTextBox);
            UIs7.Add(GradeTextBox);

            Text label3 = new Text("Буква: ", Fonts[0], 14);
            label3.FillColor = Color.Black;
            label3.Position = new Vector2f(690, 100);
            layerclasses.Objects.Add(label3);

            LetterTextBox = new TextBox(new Vector2f(750, 97), new Vector2f(40, 25));
            layerclasses.Objects.Add(LetterTextBox);
            UIs7.Add(LetterTextBox);

            Text label4 = new Text("Классный руководитель: ", Fonts[0], 14);
            label4.FillColor = Color.Black;
            label4.Position = new Vector2f(690, 130);
            layerclasses.Objects.Add(label4);

            ClassTeacher = new TextBox(new Vector2f(690, 160), new Vector2f(270, 25));
            layerclasses.Objects.Add(ClassTeacher);
            UIs7.Add(ClassTeacher);

            Text label5 = new Text("Предметы: ", Fonts[0], 14);
            label5.FillColor = Color.Black;
            label5.Position = new Vector2f(690, 190);
            layerclasses.Objects.Add(label5);

            #region Teachers List
            SubsCheckBoxes = new List<CheckBox>();
            SubsTextBoxes = new List<TextBox>();
            for (int i = 0; i < Teachers.Count; i++)
            {
                CheckBox cb = new CheckBox();
                TextBox tb = new TextBox();

                string shortsub = "";

                if (Teachers[i].Subject.Length > 10)
                {
                    shortsub = Teachers[i].Subject.Remove(10);
                    shortsub += ".";
                }
                else
                {
                    shortsub = Teachers[i].Subject;
                }

                string shortname = "";

                if (Teachers[i].Name.Length > 9)
                {
                    shortname = Teachers[i].Name.Remove(9);
                    shortname += ".";
                }
                else
                {
                    shortname = Teachers[i].Name;
                }

                if (i < 6)
                {
                    cb.Position = new Vector2f(700, 225 + i * 80);
                    cb.Checked = false;

                    SubsCheckBoxes.Add(cb);
                    UIs7.Add(cb);
                    layerclasses.Objects.Add(cb);

                    Text tchrlabel = new Text("Предмет: " + shortsub + "\nПреподаватель: " + shortname, Fonts[0], 14);
                    tchrlabel.FillColor = Color.Black;
                    tchrlabel.Position = new Vector2f(730, 220 + i * 80);
                    layerclasses.Objects.Add(tchrlabel);

                    Text tchrlabel2 = new Text("Часов в неделю:", Fonts[0], 14);
                    tchrlabel2.FillColor = Color.Black;
                    tchrlabel2.Position = new Vector2f(730, 260 + i * 80);
                    layerclasses.Objects.Add(tchrlabel2);

                    tb = new TextBox(new Vector2f(860, 258 + i * 80), new Vector2f(40, 25));
                    SubsTextBoxes.Add(tb);
                    layerclasses.Objects.Add(tb);
                    UIs7.Add(tb);

                }
                else
                {
                    cb.Position = new Vector2f(970, 225 + i * 80 - 480);
                    cb.Checked = false;

                    SubsCheckBoxes.Add(cb);
                    UIs7.Add(cb);
                    layerclasses.Objects.Add(cb);

                    Text tchrlabel = new Text("Предмет: " + shortsub + "\nПреподаватель: " + shortname, Fonts[0], 14);
                    tchrlabel.FillColor = Color.Black;
                    tchrlabel.Position = new Vector2f(1000, 220 + i * 80 - 480);
                    layerclasses.Objects.Add(tchrlabel);

                    Text tchrlabel2 = new Text("Часов в неделю:", Fonts[0], 14);
                    tchrlabel2.FillColor = Color.Black;
                    tchrlabel2.Position = new Vector2f(1000, 260 + i * 80 - 480);
                    layerclasses.Objects.Add(tchrlabel2);

                    tb = new TextBox(new Vector2f(1130, 258 + i * 80 - 480), new Vector2f(40, 25));
                    SubsTextBoxes.Add(tb);
                    layerclasses.Objects.Add(tb);
                    UIs7.Add(tb);
                }
            }
            #endregion

            Button applyButton = new Button(new Vector2f(975, 152), new Text("Добавить", Fonts[0], 14), new Vector2f(100, 40));
            applyButton.action = AddClass;
            UIs7.Add(applyButton);
            layerclasses.Objects.Add(applyButton);

            Button applyAllButton = new Button(new Vector2f(1130, 50), new Text("Применить", Fonts[0], 14), new Vector2f(100, 40), ApplyClasses);
            UIs7.Add(applyAllButton);
            layerclasses.Objects.Add(applyAllButton);

            Layers7.Add(layerclasses);

            ClassesAdding.Closed += ClassesAdding_Closed;
            ClassesAdding.KeyPressed += ClassesAdding_KeyPressed;
            ClassesAdding.MouseButtonPressed += ClassesAdding_MouseButtonPressed;
            ClassesAdding.MouseButtonReleased += ClassesAdding_MouseButtonReleased;
            ClassesAdding.MouseMoved += ClassesAdding_MouseMoved;
            ClassesAdding.MouseWheelScrolled += ClassesAdding_MouseWheelScrolled;
            ClassesAdding.TextEntered += ClassesAdding_TextEntered;
            ClassesAdding.Resized += ClassesAdding_Resized;
            ClassesAdding.KeyReleased += ClassesAdding_KeyReleased;
        }

        private bool firstclassadded = false;
        private ColumnRowInfo classRow;

        public void AddClass()
        {
            UIs7.Remove(ClassesTab);
            layerclasses.Objects.Remove(ClassesTab);

            classRow = new ColumnRowInfo(GradeTextBox.text + " " + LetterTextBox.text, new Vector2f(52, 20));
            
            if (firstclassadded)
            {
                ClassesTabRows.Add(classRow);
            }
            else
            {
                ClassesTabRows[0] = classRow;
                firstclassadded = true;
            }

            Element classtchr = new Element(new Vector2f(102, 70 + (ClassesTabRows.Count - 1) * 20), new Text(ClassTeacher.text, Fonts[0], 14), new Vector2f(270, 20));
            classtchr.Dragable = false;
            ClassesTabElements.Add(classtchr);

            string substext = "";
            
            List<Subject> subsforlist = new List<Subject>();

            for (int i = 0; i < SubsCheckBoxes.Count; i++)
            {
                if (SubsCheckBoxes[i].Checked)
                {
                    subsforlist.Add(new Subject(Teachers[i].Subject, Teachers[i], Convert.ToInt32(SubsTextBoxes[i].text)));
                    
                    if (i < SubsCheckBoxes.Count - 1)
                    {
                        if (Teachers[i].Subject.Length > 3)
                        {
                            substext += Teachers[i].Subject.Remove(3) + "., ";
                        }
                        else
                        {
                            substext += Teachers[i].Subject + "., ";
                        }
                    }
                    else
                    {
                        if (Teachers[i].Subject.Length > 3)
                        {
                            substext += Teachers[i].Subject.Remove(3) + ".";
                        }
                        else
                        {
                            substext += Teachers[i].Subject + ".";
                        }
                    }
                }
            }

            StudyingClassesList.Add(new StudyingClass(GradeTextBox.text, LetterTextBox.text, subsforlist));

            Element classsubs = new Element(new Vector2f(372, 70 + (ClassesTabRows.Count - 1) * 20), new Text(substext, Fonts[0], 14), new Vector2f(270, 20));
            classsubs.Dragable = false;
            ClassesTabElements.Add(classsubs);

            ClassesTab = new TabWithElements(ClassesTabColumns, ClassesTabRows, new Vector2f(50, 50), ClassesTabElements);

            UIs7.Add(ClassesTab);
            layerclasses.Objects.Add(ClassesTab);

            GradeTextBox.text = "";
            LetterTextBox.text = "";
            ClassTeacher.text = "";

            foreach (CheckBox cb in SubsCheckBoxes)
            {
                cb.Checked = false;
            }
        }

        private void ApplyClasses()
        {
            ClassesAdding.Close();
        }

        public void UpdateClassesForm()
        {
            DeltaTime7 = Dtime7.ElapsedTime.AsSeconds();
            Dtime7.Restart();

            App.DispatchEvents();
            ClassesAdding.DispatchEvents();
            ClassesAdding.Clear(Background);

            try
            {
                foreach (UI_Element ui in UIs7)
                {
                    ui.Update(DeltaTime7);
                }
                foreach (Layer l in Layers7)
                {
                    ClassesAdding.Draw(l);
                }
            }
            catch { }

            ClassesAdding.Display();
        }

        List<UI_Element> UIs8;
        List<Layer> Layers8;
        public void AppInfoInitialize()
        {
            ContextSettings settings = new ContextSettings(1, 0, 4);
            AppInfo = new RenderWindow(new VideoMode(520, 410), "Info", Styles.Default, settings);
            AppInfo.SetFramerateLimit(60);

            Image icon = new Image("5.png");
            AppInfo.SetIcon(512, 512, icon.Pixels);

            UIs8 = new List<UI_Element>();
            Layers8 = new List<Layer>();
            Layer lr8 = new Layer();

            Text info = new Text("StudySchedule – Программное обеспечение, \nпозволяющее " +
                "автоматизировать отдельные \nпроцессы при " +
                "составления учебного расписания.\n\n" +
                "Версия: 1.1\n\n" +
                "Релиз: 23.01.2023", Fonts[0], 14);
            info.Position = new Vector2f(50, 60);
            info.FillColor = Color.Black;
            lr8.Objects.Add(info);

            Sprite logo = new Sprite(new Texture("scaled logo.png"));
            logo.Position = new Vector2f(50, 210);
            //logo.Scale = new Vector2f(0.35f, 0.35f);
            lr8.Objects.Add(logo);

            Button infoOkButton = new Button(new Vector2f(370, 320), new Text("ОК", Fonts[0], 14), new Vector2f(100, 40));
            infoOkButton.action = AppInfo.Close;
            UIs8.Add(infoOkButton);
            lr8.Objects.Add(infoOkButton);

            Layers8.Add(lr8);

            AppInfo.Closed += AppInfo_Closed;
            AppInfo.KeyPressed += AppInfo_KeyPressed;
            AppInfo.MouseButtonPressed += AppInfo_MouseButtonPressed;
            AppInfo.MouseButtonReleased += AppInfo_MouseButtonReleased;
            AppInfo.MouseMoved += AppInfo_MouseMoved;
            AppInfo.MouseWheelScrolled += AppInfo_MouseWheelScrolled;
            AppInfo.TextEntered += AppInfo_TextEntered;
            AppInfo.Resized += AppInfo_Resized;
            AppInfo.KeyReleased += AppInfo_KeyReleased;
        }
        
        public void AppInfoUpdate()
        {
            DeltaTime8 = Dtime8.ElapsedTime.AsSeconds();
            Dtime8.Restart();

            App.DispatchEvents();
            AppInfo.DispatchEvents();
            
            AppInfo.Clear(Background);

            try
            {
                foreach (UI_Element ui in UIs8)
                {
                    ui.Update(DeltaTime8);
                }
                foreach (Layer l in Layers8)
                {
                    AppInfo.Draw(l);
                }
            }
            catch { }

            AppInfo.Display();
        }

        private void AppInfo_KeyReleased(object sender, KeyEventArgs e)
        {
            
        }

        private void AppInfo_Resized(object sender, SizeEventArgs e)
        {
            AppInfo.SetView(new View(new FloatRect(0, 0, e.Width, e.Height)));
        }

        private void AppInfo_TextEntered(object sender, TextEventArgs e)
        {
            foreach (UI_Element ui in UIs8)
                ui.TextEntered(e);
        }

        private void AppInfo_MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
        {
            foreach (UI_Element ui in UIs8)
                ui.MouseWheel(e.Delta);
        }

        private void AppInfo_MouseMoved(object sender, MouseMoveEventArgs e)
        {
            foreach (UI_Element ui in UIs8)
                ui.MouseCheck(e);
        }

        private void AppInfo_MouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            foreach (UI_Element ui in UIs8)
                ui.MouseRelease(e);
        }

        private void AppInfo_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            foreach (UI_Element ui in UIs8)
                ui.MouseClick(e);
        }

        private void AppInfo_KeyPressed(object sender, KeyEventArgs e)
        {
            foreach (UI_Element ui in UIs8)
                ui.KeyPressed(e);
        }

        private void AppInfo_Closed(object sender, EventArgs e)
        {
            AppInfo.Close();
        }

        private void ClassesAdding_KeyReleased(object sender, KeyEventArgs e)
        {

        }

        private void ClassesAdding_Resized(object sender, SizeEventArgs e)
        {
            foreach (UI_Element ui in UIs7)
            {
                ui.Resized(e);
            }
            ClassesAdding.SetView(new View(new FloatRect(0, 0, e.Width, e.Height)));
        }

        private void ClassesAdding_TextEntered(object sender, TextEventArgs e)
        {
            foreach (UI_Element ui in UIs7)
            {
                ui.TextEntered(e);
            }
        }

        private void ClassesAdding_MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
        {
            foreach (UI_Element ui in UIs7)
            {
                ui.MouseWheel(e.Delta);
            }
        }

        private void ClassesAdding_MouseMoved(object sender, MouseMoveEventArgs e)
        {
            foreach (UI_Element ui in UIs7)
            {
                ui.MouseCheck(e);
            }
        }

        private void ClassesAdding_MouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            foreach (UI_Element ui in UIs7)
            {
                ui.MouseRelease(e);
            }
        }

        private void ClassesAdding_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            foreach (UI_Element ui in UIs7)
            {
                ui.MouseClick(e);
            }
        }

        private void ClassesAdding_KeyPressed(object sender, KeyEventArgs e)
        {
            foreach (UI_Element ui in UIs7)
            {
                ui.KeyPressed(e);
            }
        }

        private void ClassesAdding_Closed(object sender, EventArgs e)
        {
            ClassesAdding.Close();
        }

        private void TeacherSchedule_KeyReleased(object sender, KeyEventArgs e)
        {
            
        }

        private void TeacherSchedule_Resized(object sender, SizeEventArgs e)
        {
            foreach (UI_Element ui in UIs6)
            {
                ui.Resized(e);
            }
            TeacherSchedule.SetView(new View(new FloatRect(0, 0, e.Width, e.Height)));
        }

        private void TeacherSchedule_TextEntered(object sender, TextEventArgs e)
        {
            foreach (UI_Element ui in UIs6)
            {
                ui.TextEntered(e);
            }
        }

        private void TeacherSchedule_MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
        {
            foreach (UI_Element ui in UIs6)
            {
                ui.MouseWheel(e.Delta);
            }
        }

        private void TeacherSchedule_MouseMoved(object sender, MouseMoveEventArgs e)
        {
            foreach (UI_Element ui in UIs6)
            {
                ui.MouseCheck(e);
            }
        }

        private void TeacherSchedule_MouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            foreach (UI_Element ui in UIs6)
            {
                ui.MouseRelease(e);
            }
        }

        private void TeacherSchedule_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            foreach (UI_Element ui in UIs6)
            {
                ui.MouseClick(e);
            }
        }

        private void TeacherSchedule_KeyPressed(object sender, KeyEventArgs e)
        {
            foreach (UI_Element ui in UIs6)
            {
                ui.KeyPressed(e);
            }
        }

        private void TeacherSchedule_Closed(object sender, EventArgs e)
        {
            TeacherSchedule.Close();
        }

        private void TeachersEditing_KeyReleased(object sender, KeyEventArgs e)
        {
            
        }

        private void TeachersEditing_Resized(object sender, SizeEventArgs e)
        {
            foreach (UI_Element ui in UIs5)
            {
                ui.Resized(e);
            }
            TeachersEditing.SetView(new View(new FloatRect(0, 0, e.Width, e.Height)));
        }

        private void TeachersEditing_TextEntered(object sender, TextEventArgs e)
        {
            foreach (UI_Element ui in UIs5)
            {
                ui.TextEntered(e);
            }
        }

        private void TeachersEditing_MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
        {
            foreach (UI_Element ui in UIs5)
            {
                ui.MouseWheel(e.Delta);
            }
        }

        private void TeachersEditing_MouseMoved(object sender, MouseMoveEventArgs e)
        {
            foreach (UI_Element ui in UIs5)
            {
                ui.MouseCheck(e);
            }
        }

        private void TeachersEditing_MouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            foreach (UI_Element ui in UIs5)
            {
                ui.MouseRelease(e);
            }
        }

        private void TeachersEditing_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            foreach (UI_Element ui in UIs5)
            {
                ui.MouseClick(e);
            }
        }

        private void TeachersEditing_KeyPressed(object sender, KeyEventArgs e)
        {
            foreach (UI_Element ui in UIs5)
            {
                ui.KeyPressed(e);
            }
        }

        private void TeachersEditing_Closed(object sender, EventArgs e)
        {
            TeachersEditing.Close();
        }

        private void WeekendsEditing_KeyReleased(object sender, KeyEventArgs e)
        {
            
        }

        private void WeekendsEditing_Resized(object sender, SizeEventArgs e)
        {
            foreach (UI_Element ui in UIs3)
            {
                ui.Resized(e);
            }
            WeekendsEditing.SetView(new View(new FloatRect(0, 0, e.Width, e.Height)));
        }

        private void WeekendsEditing_TextEntered(object sender, TextEventArgs e)
        {
            foreach (UI_Element ui in UIs3)
            {
                ui.TextEntered(e);
            }
        }

        private void WeekendsEditing_MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
        {
            foreach (UI_Element ui in UIs3)
            {
                ui.MouseWheel(e.Delta);
            }
        }

        private void WeekendsEditing_MouseMoved(object sender, MouseMoveEventArgs e)
        {
            foreach (UI_Element ui in UIs3)
            {
                ui.MouseCheck(e);
            }
        }

        private void WeekendsEditing_MouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            foreach (UI_Element ui in UIs3)
            {
                ui.MouseRelease(e);
            }
        }

        private void WeekendsEditing_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            foreach (UI_Element ui in UIs3)
            {
                ui.MouseClick(e);
            }
        }

        private void WeekendsEditing_KeyPressed(object sender, KeyEventArgs e)
        {
            foreach (UI_Element ui in UIs3)
            {
                ui.KeyPressed(e);
            }
        }

        private void WeekendsEditing_Closed(object sender, EventArgs e)
        {
            WeekendsEditing.Close();
        }

        private void RingsEditing_KeyReleased(object sender, KeyEventArgs e)
        {
            
        }

        private void RingsEditing_Resized(object sender, SizeEventArgs e)
        {
            foreach (UI_Element ui in UIs4)
            {
                ui.Resized(e);
            }
            RingsEditing.SetView(new View(new FloatRect(0, 0, e.Width, e.Height)));
        }

        private void RingsEditing_TextEntered(object sender, TextEventArgs e)
        {
            foreach (UI_Element ui in UIs4)
            {
                ui.TextEntered(e);
            }
        }

        private void RingsEditing_MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
        {
            foreach (UI_Element ui in UIs4)
            {
                ui.MouseWheel(e.Delta);
            }
        }

        private void RingsEditing_MouseMoved(object sender, MouseMoveEventArgs e)
        {
            foreach (UI_Element ui in UIs4)
            {
                ui.MouseCheck(e);
            }
        }

        private void RingsEditing_MouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            foreach (UI_Element ui in UIs4)
            {
                ui.MouseRelease(e);
            }
        }

        private void RingsEditing_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            foreach (UI_Element ui in UIs4)
            {
                ui.MouseClick(e);
            }
        }

        private void RingsEditing_KeyPressed(object sender, KeyEventArgs e)
        {
            foreach (UI_Element ui in UIs4)
            {
                ui.KeyPressed(e);
            }
        }

        private void RingsEditing_Closed(object sender, EventArgs e)
        {
            RingsEditing.Close();
        }

        private void OrganizationEntering_KeyReleased(object sender, KeyEventArgs e)
        {
            
        }

        private void OrganizationEntering_Resized(object sender, SizeEventArgs e)
        {
            OrganizationEntering.SetView(new View(new FloatRect(0, 0, e.Width, e.Height)));
        }

        private void OrganizationEntering_TextEntered(object sender, TextEventArgs e)
        {
            foreach (UI_Element ui in UIs2)
            {
                ui.TextEntered(e);
            }
        }

        private void OrganizationEntering_MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
        {
            foreach (UI_Element ui in UIs2)
            {
                ui.MouseWheel(e.Delta);
            }
        }

        private void OrganizationEntering_MouseMoved(object sender, MouseMoveEventArgs e)
        {
            foreach (UI_Element ui in UIs2)
            {
                ui.MouseCheck(e);
            }
        }

        private void OrganizationEntering_MouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            foreach (UI_Element ui in UIs2)
            {
                ui.MouseRelease(e);
            }
        }

        private void OrganizationEntering_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            foreach (UI_Element ui in UIs2)
            {
                ui.MouseClick(e);
            }
        }

        private void OrganizationEntering_KeyPressed(object sender, KeyEventArgs e)
        {
            foreach (UI_Element ui in UIs2)
            {
                ui.KeyPressed(e);
            }
        }

        private void OrganizationEntering_Closed(object sender, EventArgs e)
        {
            OrganizationEntering.Close();
        }

        public void DeleteAllSubs()
        {
            elements.Clear();
            Debug.DisplayedString = elements.Count.ToString();
        }

        private void App_KeyReleased(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.E)
                FPSDraw = !FPSDraw;
        }

        public void Close()
        {
            Console.LogToFile();
            App.Close();
        }

        private void App_Resized(object sender, SizeEventArgs e)
        {
            foreach (UI_Element ui in UIs)
                ui.Resized(e);

            App.SetView(new View(new FloatRect(0, 0, e.Width, e.Height)));

            welcome.Position = new Vector2f(App.GetView().Center.X - welcome.GetGlobalBounds().Width / 2, App.GetView().Center.Y - welcome.GetGlobalBounds().Height / 2 - e.Height / 25);
            Create.Position = new Vector2f(App.GetView().Center.X - 92, App.GetView().Center.Y + e.Height / 18);
        }

        private void App_TextEntered(object sender, TextEventArgs e)
        {
            foreach (UI_Element ui in UIs)
            {
                ui.TextEntered(e);
            }
        }

        private void App_MouseWheelScrolled(object sender, MouseWheelScrollEventArgs e)
        {
            foreach (UI_Element ui in UIs)
            {
                ui.MouseWheel(e.Delta);
            }
        }

        private void App_MouseMoved(object sender, MouseMoveEventArgs e)
        {
            foreach (UI_Element ui in UIs)
            {
                ui.MouseCheck(e);
            }
            
            foreach (GameObject go in gameObjects)
            {
                go.MoveToMouse(e, Camera);
            }
        }

        private void App_MouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            foreach (UI_Element ui in UIs)
            {
                ui.MouseRelease(e);
            }
        }

        private void App_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            foreach (UI_Element ui in UIs)
            {
                ui.MouseClick(e);
            }

            //foreach (GameObject go in gameObjects)
            //{
            //    go.Click(e, Camera);
            //}
        }

        private void App_KeyPressed(object sender, KeyEventArgs e)
        {
            foreach (UI_Element ui in UIs)
            {
                ui.KeyPressed(e);
            }
        }

        private void App_Closed(object sender, EventArgs e)
        {
            Console.LogToFile();
            App.Close();
            if (OrganizationEntering != null)
                OrganizationEntering.Close();
            if (RingsEditing != null)
                RingsEditing.Close();
            if (TeacherSchedule != null)
                TeacherSchedule.Close();
            if (WeekendsEditing != null)
                WeekendsEditing.Close();
            if (TeachersEditing != null)
                TeachersEditing.Close();
            if (AppInfo != null)
                AppInfo.Close();
        }
    }
}
