﻿using BookingApp.Domain.Model;
using BookingApp.Domain.RepositoryInterfaces;
using BookingApp.Repository;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace BookingApp.WPF.View.Guest
{
    public partial class GuestOverview : Page, INotifyPropertyChanged
    {
        private AccommodationReservationRepository _accommodationReservationRepository = new AccommodationReservationRepository();
        private ReservationModificationRequestRepository _reservationModificationRequestRepository = new ReservationModificationRequestRepository();
        public static ObservableCollection<Accommodation> Accommodations { get; set; }
        public List<Location> locations { get; set; }
        public string SelectedLocation { get; set; }
        public User LoggedInGuest { get; set; }
        public ICommand ToggleTypeCommand { get; set; }
        public List<string> selectedTypes { get; set; } = new List<string>();
        private readonly AccommodationRepository _accommodationRepository;
        private readonly LocationRepository _locationRepository;
        public GuestOverview(User guest)
        {
            InitializeComponent();
            DataContext = this;
            LoggedInGuest = guest;
            _accommodationRepository = new AccommodationRepository();
            Accommodations = new ObservableCollection<Accommodation>(_accommodationRepository.GetAll());
            _locationRepository = new LocationRepository();
            locations = _locationRepository.GetAll();
            ButtonSearch.Click += ButtonSearch_Click;
            ToggleTypeCommand = new DelegateCommand<string>(ToggleType);
            GuestsTextBox.Text = "0";
            DaysTextBox.Text = "0";

            CheckReservationModificationRequests();
        }

        private void CheckReservationModificationRequests()
        {
            var userReservations = _accommodationReservationRepository.GetByUser(LoggedInGuest);

            foreach (var reservation in userReservations)
            {
                var modificationRequest = _reservationModificationRequestRepository.GetByReservationId(reservation.Id);

                if (modificationRequest != null && modificationRequest.Status != ReservationModificationRequest.RequestStatus.PENDING)
                {
                    string message = $"Status of your reservation has been changed!.";
                    MessageBox.Show(message, "Reservation Status Change", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void TypeToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (TypePopup.IsOpen)
            {
                TypePopup.IsOpen = false;
            }
            else
            {
                TypePopup.IsOpen = true;
            }
        }

        private void ToggleType(object parameter)
        {
            string type = parameter as string;
            if (selectedTypes.Contains(type))
                selectedTypes.Remove(type);
            else
                selectedTypes.Add(type);
        }
        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text.Trim();
            string locationString = SelectedLocation;
            int locationId = -1;
            int guests = Convert.ToInt32(GuestsTextBox.Text);
            int days = Convert.ToInt32(DaysTextBox.Text);
            List<Accommodation> filteredAccommodations = new List<Accommodation>();

            foreach (var loc in locations)
            {
                if (loc.ToString().Equals(locationString))
                    locationId = loc.Id;
            }
            if (selectedTypes.Any())
            {
                foreach (var type in selectedTypes)
                {
                    var accommodationsOfType = Accommodations.Where(accommodation => FindAccommodation(accommodation, name, locationId, type, guests, days));
                    filteredAccommodations.AddRange(accommodationsOfType);
                }
            }
            else
            {
                var accommodations = Accommodations.Where(accommodation => FindAccommodation(accommodation, name, locationId, null, guests, days));
                filteredAccommodations.AddRange(accommodations);
            }
            AccommodationsDataGrid.ItemsSource = filteredAccommodations;
        }
        private bool FindAccommodation(Accommodation accommodation, string name, int locationId, string type, int guests, int days)
        {
            return (string.IsNullOrWhiteSpace(name) || accommodation.Name.ToLower().Contains(name.ToLower())) &&
                    (type == null || accommodation.Type.ToString().ToLower().Equals(type.ToLower())) &&
                    (guests <= 0 || accommodation.MaxGuests >= guests) &&
                    (days <= 0 || accommodation.MinReservations <= days) &&
                    (accommodation.LocationId == locationId || locationId == -1);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void CheckBox_LostFocus(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            string type = checkBox.Content.ToString();

            if (checkBox.IsChecked == true && !selectedTypes.Contains(type))
                selectedTypes.Add(type);
            else if (checkBox.IsChecked == false && selectedTypes.Contains(type))
                selectedTypes.Remove(type);
        }
        private void ReserveAccommodation_Click(object sender, RoutedEventArgs e)
        {
            var selectedAccommodation = (sender as Button)?.DataContext as Accommodation;
            AccommodationReservationForm reservationAccommodationWindow = new AccommodationReservationForm(selectedAccommodation, LoggedInGuest);
            reservationAccommodationWindow.Show();
        }

    }
}
