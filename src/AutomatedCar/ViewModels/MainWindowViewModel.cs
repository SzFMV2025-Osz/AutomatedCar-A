namespace AutomatedCar.ViewModels
{
    using AutomatedCar.Models;
    using AutomatedCar.SystemComponents.InputHandling;
    using ExCSS;
    using ReactiveUI;

    public class MainWindowViewModel : ViewModelBase
    {
        private DashboardViewModel dashboard;
        private CourseDisplayViewModel courseDisplay;
        private KeyboardHandler keyboardhandler;

        public MainWindowViewModel(World world)
        {
            this.CourseDisplay = new CourseDisplayViewModel(world);
            this.Dashboard = new DashboardViewModel(world.ControlledCar);
            this.keyboardhandler=new KeyboardHandler(world.ControlledCar.VirtualFunctionBus);
        }

        public CourseDisplayViewModel CourseDisplay
        {
            get => this.courseDisplay;
            private set => this.RaiseAndSetIfChanged(ref this.courseDisplay, value);
        }

        public DashboardViewModel Dashboard
        {
            get => this.dashboard;
            private set => this.RaiseAndSetIfChanged(ref this.dashboard, value);
        }
        public KeyboardHandler KeyboardHandler
        {
            get => this.keyboardhandler;
            set => this.RaiseAndSetIfChanged(ref this.keyboardhandler,value);
        }

        public void NextControlledCar()
        {
            World.Instance.NextControlledCar();
            this.KeyboardHandler = new KeyboardHandler(World.Instance.ControlledCar.VirtualFunctionBus);
            this.Dashboard = new DashboardViewModel(World.Instance.ControlledCar);
        }

        public void PrevControlledCar()
        {
            World.Instance.PrevControlledCar();
            this.KeyboardHandler = new KeyboardHandler(World.Instance.ControlledCar.VirtualFunctionBus);
            this.Dashboard = new DashboardViewModel(World.Instance.ControlledCar);
        }
    }
}