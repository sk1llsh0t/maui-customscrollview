namespace maui_test
{
    public class CustomScrollView : ScrollView
    {
        public static readonly BindableProperty ScrollToEndRequiredProperty =
             BindableProperty.Create("ScrollToEndRequired", typeof(bool), typeof(CustomScrollView), false, defaultBindingMode: BindingMode.TwoWay);
        public bool ScrollToEndRequired
        {
            get { return (bool)this.GetValue(ScrollToEndRequiredProperty); }
            set { this.SetValue(ScrollToEndRequiredProperty, value); }
        }
        public static readonly BindableProperty ScrollToEndCompletedProperty =
           BindableProperty.Create("ScrollToEndCompleted", typeof(bool), typeof(CustomScrollView), false, defaultBindingMode: BindingMode.TwoWay);
        public bool ScrollToEndCompleted
        {
            get { return (bool)this.GetValue(ScrollToEndCompletedProperty); }
            set { this.SetValue(ScrollToEndCompletedProperty, value); }
        }

        public static readonly BindableProperty DisableFlingProperty =
             BindableProperty.Create("DisableFling", typeof(bool), typeof(CustomScrollView), false, defaultBindingMode: BindingMode.TwoWay, propertyChanged: OnIsPropertyChanged);

        private static void OnIsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue != oldValue && (bool)newValue)
            {
                CustomScrollView control = (CustomScrollView)bindable;

                var UpGesture = new SwipeGestureRecognizer() { Direction = SwipeDirection.Up };
                var DownGesture = new SwipeGestureRecognizer() { Direction = SwipeDirection.Down };

                UpGesture.Swiped += control.UpDownGesture_Swiped;
                DownGesture.Swiped += control.UpDownGesture_Swiped;

                control.GestureRecognizers.Add(UpGesture);
                control.GestureRecognizers.Add(DownGesture);
            }
        }

        public bool DisableFling
        {
            get { return (bool)this.GetValue(DisableFlingProperty); }
            set { this.SetValue(DisableFlingProperty, value); }
        }

        public CustomScrollView() : base() { }

        private void UpDownGesture_Swiped(object sender, SwipedEventArgs e)
        {
            if (e.Direction == SwipeDirection.Down)
            {
                if (DisableFling)
                {
                    var animation = new Animation(
                        callback: y => ScrollToAsync(ScrollX, y, animated: false),
                        start: ScrollY,
                        end: ScrollY >= 200 ? ScrollY - 200 : 0);
                    animation.Commit(
                        owner: this,
                        name: "Scroll",
                        length: 200,
                        easing: Easing.CubicIn);
                }
            }
            else if (e.Direction == SwipeDirection.Up)
            {
                double spaceAvailableForScrolling = this.ContentSize.Height - this.Height;

                if (ScrollToEndRequired)
                {
                    double buffer = 32;
                    if (spaceAvailableForScrolling < this.ScrollY + buffer)
                    {
                        ScrollToEndCompleted = true;
                    }
                }

                if (DisableFling)
                {
                    var animation = new Animation(
                        callback: y => ScrollToAsync(ScrollX, y, animated: false),
                        start: ScrollY,
                        end: spaceAvailableForScrolling >= 200 ? ScrollY + 200 : spaceAvailableForScrolling - ScrollY);
                    animation.Commit(
                        owner: this,
                        name: "Scroll",
                        length: 200,
                        easing: Easing.CubicIn);
                }
            }
        }
    }
}
