using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Collections;
using System.Windows.Threading;

namespace iostamagotchi
{
    /// <summary>
    /// Modified version of Dispatcher that holds reference to its parent modelFlashCardStudy model
    /// 
    /// </summary>
    public class StudyDispatcherTimer : DispatcherTimer
    {
        /// <summary>
        /// Parent modelFlashCardStudy model
        /// </summary>
        public ModelFlashCardStudy ParentCard;
    }
}
