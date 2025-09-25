import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';

type RandomResponse = {
  value: number;
};

@Injectable({
  providedIn: 'root'
})
export class RandomService {
  private readonly http = inject(HttpClient);
  
  readonly randomData = signal<any>(null);
  readonly isLoading = signal<boolean>(false);
  readonly error = signal<string | null>(null);

  async fetchRandomData() {
    this.isLoading.set(true);
    this.error.set(null);
    
    try {
      const data = await firstValueFrom(
        this.http.get('http://localhost:5158/random')
      );
      this.randomData.set(data);
    } catch (err) {
      this.error.set('Failed to fetch data');
    } finally {
      this.isLoading.set(false);
    }
  }
}
