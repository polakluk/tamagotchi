using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Threading;

namespace iostamagotchi
{
    public partial class StudyingPage : PhoneApplicationPage
    {
        private Rectangle m_selectedHandler = null;
        private double m_gradientMargin = 26;
        private double m_handleTolerance = 13;
        private double m_handleCenter = 184;

        public StudyingPage()
        {
            InitializeComponent();
            this.gItem.DataContext = App.StudyViewModel.FlashCard;

            Touch.FrameReported += this.onTouchFrameReported;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            App.StudyViewModel.LoadDataFromIsolated();

            string SetId, Algorithm;
            // decide, if we should use algorithm
            if (this.NavigationContext.QueryString.TryGetValue("alg", out Algorithm))
            {
                App.StudyViewModel.UseAlgorithm = Algorithm == "1";
            }
            else
            {
                App.StudyViewModel.UseAlgorithm = App.Settings.GetValueOrDefault<bool>("study.algorithm");
            }

            if (this.NavigationContext.QueryString.TryGetValue("setId", out SetId))
            {
                App.StudyViewModel.GetStudySet(Convert.ToInt32(SetId));
            }
            else
            {
                App.StudyViewModel.GetStudySet(App.Settings.GetValueOrDefault<int>("study.setId"));
            }

            if (App.StudyViewModel.StudyingOn)
            {
                App.StudyViewModel.LoadNextCard();
            }

            if (App.StudyViewModel.UseAlgorithm)
            {
                this.PageTitle.Text = "Studying";
            }
            else
            {
                this.PageTitle.Text = "Practising";
            }
        }

        private void tbFrontSide_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
        	// TODO: Add event handler implementation here.
            TextBlock block = sender as TextBlock;
            ModelFlashCardStudy item = block.DataContext as ModelFlashCardStudy;

            switch( item.CardState )
            {
                case eFlashCardStateStudy.fcssNone:
                    {
                        item.IsBackVisible = !item.IsBackVisible;
                        item.IsHandleVisible = true;
                        this.AnimateGradientIn(block.Parent as Grid);
                        item.CardState = eFlashCardStateStudy.fcssBackSide;
                        break;
                    }
                case eFlashCardStateStudy.fcssBackSide:
                    {
                        // We expect swiping -> not tapping at this point
                        break;
                    }
            }
        }

        private void onTouchFrameReported(object sender, TouchFrameEventArgs args)
        {
            TouchPoint point = args.GetPrimaryTouchPoint(null);

            if(point == null)
            {
                return;
            }

            switch(point.Action)
            {
                case TouchAction.Down:
                    {
                        if (point.TouchDevice.DirectlyOver is Rectangle)
                        {
                            Rectangle rect = point.TouchDevice.DirectlyOver as Rectangle;
                            if ( rect.Name != "rHandle" ) // usre grabbed correct element
                                return;
                            ModelFlashCardStudy item = rect.DataContext as ModelFlashCardStudy;
                            // act only when we want to handle swiping
                            if (item.CardState != eFlashCardStateStudy.fcssBackSide)
                                return;
                            // initiate swiping for this card
                            this.m_selectedHandler = rect;
                        }
                        break;
                    }
                case TouchAction.Move:
                    {
                        if (this.m_selectedHandler != null)
                        {
                            ModelFlashCardStudy item = this.m_selectedHandler.DataContext as ModelFlashCardStudy;
                            double posX = point.Position.X - this.m_selectedHandler.Width;
                            this.CheckHandlerPosition(posX, out posX, (this.m_selectedHandler.Parent as Grid).Width, this.m_selectedHandler.Width, 0);

                            this.m_selectedHandler.Margin = new Thickness(posX,
                                                                            this.m_selectedHandler.Margin.Top,
                                                                            this.m_selectedHandler.Margin.Right,
                                                                            this.m_selectedHandler.Margin.Bottom);
                        }
                        break;
                    }
                case TouchAction.Up:
                    {
                        if (this.m_selectedHandler == null)
                            return;
                        ModelFlashCardStudy item = this.m_selectedHandler.DataContext as ModelFlashCardStudy;
                        double posX = point.Position.X - this.m_selectedHandler.Width;

                        if (this.CheckHandlerPosition(posX, out posX, (this.m_selectedHandler.Parent as Grid).Width, this.m_selectedHandler.Width, m_handleTolerance))
                        {
                            // user evaluated his answer
                            item.IsBackVisible = false;
                            item.IsCardVisible = false;
                            item.IsHandleVisible = false;
                            this.AnimateHandlerCenter(this.m_selectedHandler.Parent as Grid, 0.3);
                            this.AnimateGradientOut(this.m_selectedHandler.Parent as Grid);
                            item.CardState = eFlashCardStateStudy.fcssFadingOut;
                            item.AnswerState = this.m_handleCenter > posX ? eFlachCardAnswerState.fcasNo : eFlachCardAnswerState.fcasYes;
                            item.AnimationTimer.Start();
                        }
                        else
                        {
                            // user did not evaluate his answer
                            this.AnimateHandlerCenter(this.m_selectedHandler.Parent as Grid, 0.2);
                        }

                        this.m_selectedHandler = null;
                        break;
                    }
            }
            return;
        }

        private Boolean CheckHandlerPosition(double act, out double act_out, double gridWidth, double handlerWidth, double tolerance)
        {
            Boolean result = false;

            if (act < this.m_gradientMargin + tolerance)
            {
                act_out  = this.m_gradientMargin;
                result = true;
            }
                else if (act > (gridWidth - handlerWidth - this.m_gradientMargin - tolerance))
                {
                    act_out = gridWidth - handlerWidth - this.m_gradientMargin;
                    result = true;
                }
                    else
                    {
                        act_out = act;
                    }
            return result;
        }

        private void AnimateGradientIn(Grid gParent)
        {
            DoubleAnimation anim1 = new DoubleAnimation();
            DoubleAnimation anim2 = new DoubleAnimation();
            DoubleAnimation anim3 = new DoubleAnimation();
            anim1.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            anim1.From = 12;
            anim1.To = this.m_gradientMargin;
            anim2.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            anim2.From = 318;
            anim2.To = 264;
            anim3.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            anim3.From = 3;
            anim3.To = 112;


            Storyboard.SetTarget(anim1, (gParent.FindName("gradientMarginLeftRight") as ThicknessWrapper));
            Storyboard.SetTargetProperty(anim1, new PropertyPath("Value"));
            Storyboard.SetTarget(anim2, (gParent.FindName("gradientMarginTop") as ThicknessWrapper));
            Storyboard.SetTargetProperty(anim2, new PropertyPath("Value"));
            Storyboard.SetTarget(anim3, (gParent.FindName("rGradient") as Rectangle));
            Storyboard.SetTargetProperty(anim3, new PropertyPath("Height"));

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(anim1);
            storyboard.Children.Add(anim2);
            storyboard.Children.Add(anim3);
            storyboard.Begin();
        }

        private void AnimateGradientOut(Grid gParent)
        {
            DoubleAnimation anim1 = new DoubleAnimation();
            DoubleAnimation anim2 = new DoubleAnimation();
            DoubleAnimation anim3 = new DoubleAnimation();
            anim1.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            anim1.From = this.m_gradientMargin;
            anim1.To = 12;
            anim2.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            anim2.From = 264;
            anim2.To = 318;
            anim3.Duration = new Duration(TimeSpan.FromSeconds(0.3));
            anim3.From = 112;
            anim3.To = 3;

            Storyboard.SetTarget(anim1, (gParent.FindName("gradientMarginLeftRight") as ThicknessWrapper));
            Storyboard.SetTargetProperty(anim1, new PropertyPath("Value"));
            Storyboard.SetTarget(anim2, (gParent.FindName("gradientMarginTop") as ThicknessWrapper));
            Storyboard.SetTargetProperty(anim2, new PropertyPath("Value"));
            Storyboard.SetTarget(anim3, (gParent.FindName("rGradient") as Rectangle));
            Storyboard.SetTargetProperty(anim3, new PropertyPath("Height"));

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(anim1);
            storyboard.Children.Add(anim2);
            storyboard.Children.Add(anim3);
            storyboard.Begin();
        }

        private void AnimateHandlerCenter(Grid gParent, double duration)
        {
            // user did not evaluate his answer
            DoubleAnimation anim = new DoubleAnimation();
            anim.Duration = new Duration(TimeSpan.FromSeconds(duration));
            anim.From = this.m_selectedHandler.Margin.Left;
            anim.To = this.m_handleCenter;
            Storyboard.SetTarget(anim, (gParent.FindName("handleMarginLeft") as ThicknessWrapper));
            Storyboard.SetTargetProperty(anim, new PropertyPath("Value"));
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(anim);
            storyboard.Begin();
        }
    }
}
