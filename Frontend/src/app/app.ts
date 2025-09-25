import { Component, inject, OnInit } from '@angular/core';
import { RandomService } from './services/random.service';

@Component({
  selector: 'app-root',
  standalone: true,
  template: `
    <main>
      <h1>Random Number Generator</h1>

      @if (loading()) {
        <p>Loading...</p>
      } @else if (error()) {
        <p class="error">{{ error() }}</p>
      } @else if (data()) {
        <p data-testid="random-value">Current random value: {{ data().value }}</p>
      }

      <button type="button" (click)="fetchData()" [disabled]="loading()">
        Fetch another
      </button>
    </main>
  `,
  styleUrl: './app.css'
})
export class App implements OnInit {
  private readonly randomService = inject(RandomService);

  readonly data = this.randomService.randomData;
  readonly loading = this.randomService.isLoading;
  readonly error = this.randomService.error;

  ngOnInit() {
    this.fetchData();
  }
  
  fetchData() {
    this.randomService.fetchRandomData();
  }
}
