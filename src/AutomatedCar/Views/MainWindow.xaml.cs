namespace AutomatedCar.Views
{
    using AutomatedCar.ViewModels;
    using Avalonia.Controls;
    using Avalonia.Input;
    using Avalonia.Markup.Xaml;

    public class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            Keyboard.Keys.Add(e.Key);
            base.OnKeyDown(e);

            MainWindowViewModel viewModel = (MainWindowViewModel)this.DataContext;

            if (Keyboard.IsKeyDown(Key.Up))
            {
                viewModel.KeyboardHandler.HandleKeyDown_Up();
            }

            if (Keyboard.IsKeyDown(Key.Down))
            {
                viewModel.KeyboardHandler.HandleKeyDown_Down();
            }

            if (Keyboard.IsKeyDown(Key.Left))
            {
                viewModel.KeyboardHandler.HandleKeyDown_Left();
            }

            if (Keyboard.IsKeyDown(Key.Right))
            {
                viewModel.KeyboardHandler.HandleKeyDown_Right();
            }

            if (Keyboard.IsKeyDown(Key.Q))
            {
                viewModel.KeyboardHandler.HandleKeyDown_Q();
            }

            if (Keyboard.IsKeyDown(Key.A))
            {
                viewModel.KeyboardHandler.HandleKeyDown_A();
            }

            if (Keyboard.IsKeyDown(Key.PageUp))
            {
                viewModel.CourseDisplay.PageUp();
            }

            if (Keyboard.IsKeyDown(Key.PageDown))
            {
                viewModel.CourseDisplay.PageDown();
            }

            if (Keyboard.IsKeyDown(Key.D1))
            {
                viewModel.CourseDisplay.ToggleDebug();
            }

            if (Keyboard.IsKeyDown(Key.D2))
            {
                viewModel.CourseDisplay.ToggleCamera();
            }

            if (Keyboard.IsKeyDown(Key.D3))
            {
                viewModel.CourseDisplay.ToggleRadar();
            }

            if (Keyboard.IsKeyDown(Key.D4))
            {
                viewModel.CourseDisplay.ToggleUltrasonic();
            }

            if (Keyboard.IsKeyDown(Key.D5))
            {
                viewModel.CourseDisplay.ToggleRotation();
            }

            if (Keyboard.IsKeyDown(Key.F1))
            {
                new HelpWindow().Show();
                Keyboard.Keys.Remove(Key.F1);
            }

            if (Keyboard.IsKeyDown(Key.F5))
            {
                viewModel.NextControlledCar();
                viewModel.KeyboardHandler.ResetAllValues();
                Keyboard.Keys.Remove(Key.F5);
            }

            if (Keyboard.IsKeyDown(Key.F6))
            {
                viewModel.PrevControlledCar();
                viewModel.KeyboardHandler.ResetAllValues();
                Keyboard.Keys.Remove(Key.F5);
            }

            var scrollViewer = this.Get<CourseDisplayView>("courseDisplay").Get<ScrollViewer>("scrollViewer");
            viewModel.CourseDisplay.FocusCar(scrollViewer);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            MainWindowViewModel viewModel = (MainWindowViewModel)this.DataContext;
            if (e.Key == Key.Up)
            {
                viewModel.KeyboardHandler.HandleKeyUp_Up();
            }
            else if (e.Key == Key.Down)
            {
                viewModel.KeyboardHandler.HandleKeyUp_Down();
            }
            else if (e.Key == Key.Left)
            {
                viewModel.KeyboardHandler.HandleKeyUp_Left();
            }
            else if (e.Key == Key.Right)
            {
                viewModel.KeyboardHandler.HandleKeyUp_Right();
            }

            base.OnKeyUp(e);

            Keyboard.Keys.Remove(e.Key);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}