﻿using Hearthstone_Deck_Tracker;
using PackTracker.Controls;
using PackTracker.Storage;
using PackTracker.View.Cache;
using System;
using System.Windows;

namespace PackTracker
{
    internal class WindowManager
    {
        private string _name;
        private Window _pityWin, _statisticWin, _historyWin, _logWin, _searchWin, _pityOverlay, _manualInsertWin;

        public WindowManager(string name)
        {
            this._name = name;
        }

        public void ShowPityWin(Settings Settings, History History, PityTimerRepository PityTimers)
        {
            if (this._pityWin == null)
            {
                this._pityWin = new Controls.PityTimer.PityTimer(History, PityTimers, Settings)
                {
                    Owner = Hearthstone_Deck_Tracker.Core.MainWindow,
                };
                this._pityWin.Closed += (sender, e) => this._pityWin = null;
                this._pityWin.Loaded += (sender, e) => this._pityWin.Title = this._name + ": " + this._pityWin.Title;

                this._pityWin.Show();
            }

            this._pityWin.Focus();
        }

        public void ShowStatisticWin(Settings Settings, History History)
        {
            if (this._statisticWin == null)
            {
                this._statisticWin = new Statistic(History, Settings)
                {
                    Owner = Hearthstone_Deck_Tracker.Core.MainWindow,
                };
                this._statisticWin.Closed += (sender, e) => this._statisticWin = null;
                this._statisticWin.Loaded += (sender, e) => this._statisticWin.Title = this._name + ": " + this._statisticWin.Title;

                this._statisticWin.Show();
            }

            this._statisticWin.Focus();
        }

        public void ShowHistoryWin(History History)
        {
            if (this._historyWin == null)
            {
                this._historyWin = new Controls.History(History, new HistoryDatePicker(History))
                {
                    Owner = Hearthstone_Deck_Tracker.Core.MainWindow,
                };
                this._historyWin.Closed += (sender, e) => this._historyWin = null;
                this._historyWin.Loaded += (sender, e) => this._historyWin.Title = this._name + ": " + this._historyWin.Title;

                this._historyWin.Show();
            }

            this._historyWin.Focus();
        }

        public void ShowLogWin(History History)
        {
            if (this._logWin == null)
            {
                this._logWin = new Log(History)
                {
                    Owner = Hearthstone_Deck_Tracker.Core.MainWindow,
                };
                this._logWin.Closed += (sender, e) => this._logWin = null;
                this._logWin.Loaded += (sender, e) => this._logWin.Title = this._name + ": " + this._logWin.Title;

                this._logWin.Show();
            }

            this._logWin.Focus();
        }

        public void ShowSearchWin(History History)
        {
            if (this._searchWin == null)
            {
                this._searchWin = new Search(History)
                {
                    Owner = Hearthstone_Deck_Tracker.Core.MainWindow,
                };
                this._searchWin.Closed += (sender, e) => this._searchWin = null;
                this._searchWin.Loaded += (sender, e) => this._searchWin.Title = this._name + ": " + this._searchWin.Title;

                this._searchWin.Show();
            }

            this._searchWin.Focus();
        }

        public void ShowManualInsertWin(History History)
        {
            if (this._manualInsertWin == null)
            {
                this._manualInsertWin = new ManualPackInsert(History)
                {
                    Owner = Hearthstone_Deck_Tracker.Core.MainWindow,
                };
                this._manualInsertWin.Closed += (sender, e) => this._manualInsertWin = null;
                this._manualInsertWin.Loaded += (sender, e) => this._manualInsertWin.Title = this._name + ": " + this._manualInsertWin.Title;

                this._manualInsertWin.Show();
            }

            this._manualInsertWin.Focus();
        }

        public void ShowSettingsWin(Settings Settings, ISettingsStorage SettingsStorage, Type PreSelection = null)
        {
            var Win = new Controls.Settings.Settings(Settings)
            {
                Owner = Hearthstone_Deck_Tracker.Core.MainWindow
            };
            Win.Closed += (sender, e) => SettingsStorage.Store(Settings);
            Win.Title = this._name + ": " + Win.Title;

            if (PreSelection != null)
            {
                foreach (var Item in Win.lb_tabs.Items)
                {
                    if (Item.GetType() == PreSelection)
                    {
                        Win.lb_tabs.SelectedItem = Item;
                        break;
                    }
                }
            }

            Win.ShowDialog();
        }

        public void ShowPityTimerOverlay(History History, PityTimerRepository PityTimers, bool rightmost)
        {
            if (this._pityOverlay == null)
            {
                this._pityOverlay = new Controls.PityTimer.PityTimerOverlay(History, PityTimers);
                Core.MainWindow.Closed += this.ClosePityTimerOverlay;
                this._pityOverlay.Closed += (sender, e) => this._pityOverlay = null;
            }

            var relativeRight = .01;
            var relativeBottom = .35;

            this._pityOverlay.Show();

            if (User32.GetHearthstoneWindow() == IntPtr.Zero || rightmost)
            {
                this._pityOverlay.Left = SystemParameters.PrimaryScreenWidth - (SystemParameters.PrimaryScreenWidth * relativeRight) - this._pityOverlay.Width;
                this._pityOverlay.Top = SystemParameters.PrimaryScreenHeight - (SystemParameters.PrimaryScreenHeight * relativeBottom) - this._pityOverlay.Height;
            }
            else
            {
                var rect = User32.GetHearthstoneRect(dpiScaling: true);
                this._pityOverlay.Left = rect.Right + (rect.Width * relativeRight) - this._pityOverlay.Width;
                this._pityOverlay.Top = rect.Bottom + (rect.Height * relativeBottom) - this._pityOverlay.Height;
            }
        }

        private void ClosePityTimerOverlay(object sender, EventArgs e)
        {
            this.ClosePityTimerOverlay();
        }

        public void ClosePityTimerOverlay()
        {
            if (this._pityOverlay != null)
            {
                this._pityOverlay.Dispatcher.Invoke(() => this._pityOverlay.Close());

                this._pityOverlay = null;
                Hearthstone_Deck_Tracker.Core.MainWindow.Closed -= this.ClosePityTimerOverlay;
            }
        }
    }
}
