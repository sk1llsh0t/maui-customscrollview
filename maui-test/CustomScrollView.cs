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

        //add ability to turn fling on or off in the custom renderer
        public static readonly BindableProperty DisableFlingProperty =
             BindableProperty.Create("DisableFling", typeof(bool), typeof(CustomScrollView), false, defaultBindingMode: BindingMode.TwoWay);
        public bool DisableFling
        {
            get { return (bool)this.GetValue(DisableFlingProperty); }
            set
            {
                this.SetValue(DisableFlingProperty, value);

                if (DisableFling)
                {
                    var UpGesture = new SwipeGestureRecognizer() { Direction = SwipeDirection.Up };
                    var DownGesture = new SwipeGestureRecognizer() { Direction = SwipeDirection.Down };

                    UpGesture.Swiped += UpDownGesture_Swiped;
                    DownGesture.Swiped += UpDownGesture_Swiped;

                    this.GestureRecognizers.Add(UpGesture);
                }
            }
        }

        public CustomScrollView() : base() { }

        private void UpDownGesture_Swiped(object sender, SwipedEventArgs e)
        {            
            if (e.Direction == SwipeDirection.Down)
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
                        end: spaceAvailableForScrolling >= 200 ? ScrollY - 200 : ScrollY - spaceAvailableForScrolling);
                    animation.Commit(
                        owner: this,
                        name: "Scroll",
                        length: 200,
                        easing: Easing.CubicIn);
                }
            }
            else if (e.Direction == SwipeDirection.Up)
            {
                if (DisableFling)
                {
                    var animation = new Animation(
                        callback: y => ScrollToAsync(ScrollX, y, animated: false),
                        start: ScrollY,
                        end: ScrollY >= 200 ? ScrollY - 200 : ScrollY);
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
