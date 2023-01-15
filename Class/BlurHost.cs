using System.Windows.Media.Effects;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace Index.Class
{
    public class BlurHost : ContentControl
    {
        public Visual BlurBackground
        {
            get => (Visual)GetValue(BlurBackgroundProperty);
            set => SetValue(BlurBackgroundProperty, value);
        }

        public static readonly DependencyProperty BlurBackgroundProperty =
            DependencyProperty.Register(
              "BlurBackground",
              typeof(Visual),
              typeof(BlurHost),
              new PropertyMetadata(default(Visual), OnBlurBackgroundChanged));

        public double BlurOpacity
        {
            get => (double)GetValue(BlurOpacityProperty);
            set => SetValue(BlurOpacityProperty, value);
        }

        public static readonly DependencyProperty BlurOpacityProperty =
            DependencyProperty.Register(
              "BlurOpacity",
              typeof(double),
              typeof(BlurHost),
              new PropertyMetadata(1.0));

        public BlurEffect BlurEffect
        {
            get => (BlurEffect)GetValue(BlurEffectProperty);
            set => SetValue(BlurEffectProperty, value);
        }

        public static readonly DependencyProperty BlurEffectProperty =
            DependencyProperty.Register(
              "BlurEffect",
              typeof(BlurEffect),
              typeof(BlurHost),
              new PropertyMetadata(
                new BlurEffect()
                {
                    Radius = 10,
                    KernelType = KernelType.Gaussian,
                    RenderingBias = RenderingBias.Performance
                }));

        private Border PART_BlurDecorator { get; set; }
        private VisualBrush BlurDecoratorBrush { get; set; }

        static BlurHost()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BlurHost), new FrameworkPropertyMetadata(typeof(BlurHost)));
        }

        public BlurHost()
        {
            Loaded += OnLoaded;

            BlurDecoratorBrush = new VisualBrush()
            {
                ViewboxUnits = BrushMappingMode.Absolute,
                Opacity = BlurOpacity
            };
        }

        private void DrawBlurredElementBackground()
        {
            if (!TryFindVisualRootContainer(this, out FrameworkElement rootContainer))
            {
                return;
            }

            Rect elementBounds = TransformToVisual(rootContainer)
              .TransformBounds(new Rect(RenderSize));

            BlurDecoratorBrush.Viewbox = elementBounds;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (TryFindVisualRootContainer(this, out FrameworkElement rootContainer))
            {
                rootContainer.SizeChanged += OnRootContainerElementResized;
            }

            DrawBlurredElementBackground();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_BlurDecorator = GetTemplateChild("PART_BlurDecorator") as Border;
            PART_BlurDecorator.Effect = BlurEffect;
            PART_BlurDecorator.Background = BlurDecoratorBrush;
        }

        private static void OnBlurBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var this_ = d as BlurHost;
            this_.BlurDecoratorBrush.Visual = e.NewValue as Visual;
            this_.DrawBlurredElementBackground();
        }

        private void OnRootContainerElementResized(object sender, SizeChangedEventArgs e)
          => DrawBlurredElementBackground();

        private bool TryFindVisualRootContainer(DependencyObject child, out FrameworkElement rootContainerElement)
        {
            rootContainerElement = null;
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            if (parent == null)
            {
                return false;
            }

            if (parent is not Window visualRoot)
            {
                return TryFindVisualRootContainer(parent, out rootContainerElement);
            }

            rootContainerElement = visualRoot.Content as FrameworkElement;
            return true;
        }
    }
}