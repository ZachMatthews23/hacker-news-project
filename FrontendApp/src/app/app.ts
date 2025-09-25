import { Component, signal, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { WeatherService, WeatherForecast } from './weather.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  protected readonly title = signal('FrontendApp');
  protected readonly weatherData = signal<WeatherForecast[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);

  constructor(private weatherService: WeatherService) {}

  ngOnInit() {
    this.loadWeatherData();
  }

  loadWeatherData() {
    this.loading.set(true);
    this.error.set(null);
    
    this.weatherService.getWeatherForecast().subscribe({
      next: (data) => {
        this.weatherData.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Failed to load weather data. Make sure the backend API is running.');
        this.loading.set(false);
        console.error('Error loading weather data:', err);
      }
    });
  }
}
