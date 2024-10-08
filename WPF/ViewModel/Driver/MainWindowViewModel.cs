﻿using BookingApp.Domain.Model;
using BookingApp.WPF.View.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using BookingApp.Application.UseCases;
using BookingApp.LogicServices.Driver;

namespace BookingApp.WPF.ViewModel.Driver
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private DriveReservationService driveReservationService {  get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int DriverID { get; set; }
        public User Korisnik { get; set; }

        private Visibility isNotVisible;
        public Visibility IsNotVisible
        {
            get { return isNotVisible; }
            set
            {
                isNotVisible = value;
                OnPropertyChanged(); // Notify the view of the change
            }
        }

        private LocStateUpdateService locationStateService { get; set; }

        public MainWindowViewModel(User u)
        {
            DriverID = u.Id;
            Korisnik = u;
            driveReservationService = new DriveReservationService();
            IsNotVisible = Visibility.Hidden;
            locationStateService = new LocStateUpdateService(u.Id);
            locationStateService.CheckState();
        }

        public void ShowCreateVehicleForm(object sender, RoutedEventArgs e, Page owner)
        {
            VehicleForm vehicleForm = new VehicleForm(DriverID);
            vehicleForm.VM.VehicleAdded += VehicleForm_VehicleAdded;
            owner.NavigationService.Navigate(vehicleForm);
            isNotVisible = Visibility.Hidden;
        }

        public void VehicleForm_VehicleAdded(object? sender, EventArgs e)
        {
            isNotVisible = Visibility.Visible;
            locationStateService.CheckState();
        }

        public void btnStats_Click(object sender, RoutedEventArgs e, Page owner)
        {
            Stats sForm = new Stats(DriverID, driveReservationService);
            owner.NavigationService.Navigate(sForm);
            isNotVisible = Visibility.Hidden;
        }

        public void ChangeVisibility(Visibility visibility)
        {
            IsNotVisible = visibility;
        }

        internal void btnData_Click(object sender, RoutedEventArgs e, Page currentPage)
        {
            PersonalData personalDataPage = new PersonalData();

            
            currentPage.NavigationService.Navigate(personalDataPage);
        }

        internal void btnVacatioRequest_Click(object sender, RoutedEventArgs e, Page currentPage)
        {
            VacationRequest vacationRequest = new VacationRequest(DriverID);
            currentPage.NavigationService.Navigate(vacationRequest);
        }

        internal void btnVacationReports_Click(object sender, RoutedEventArgs e, Page currentPage)
        {
            VacationReports vacationReports = new VacationReports(DriverID);
            currentPage.NavigationService.Navigate(vacationReports);
        }

        internal void btnTutorial_Click(object sender, RoutedEventArgs e, Page currentPage)
        {
            Tutorial tutorial = new Tutorial();
            currentPage.NavigationService.Navigate(tutorial);
        }
    }
}
