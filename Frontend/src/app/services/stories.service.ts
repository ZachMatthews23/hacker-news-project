import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { StoriesResponse, StorySearchRequest } from '@app/shared/types';
import { API_CONFIG } from '@app/shared/config';
import { firstValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class StoriesService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = API_CONFIG.baseUrl;
  
  readonly storiesData = signal<StoriesResponse | null>(null);
  readonly isLoading = signal<boolean>(false);
  readonly error = signal<string | null>(null);

   async fetchNewestStories(page: number = 1, pageSize: number = 20) {
    this.isLoading.set(true);
    this.error.set(null);
    
    try {
      const response = await firstValueFrom(
        this.http.get<StoriesResponse>(`${this.baseUrl}${API_CONFIG.endpoints.hackernews.newest}`, {
          params: { page: page.toString(), pageSize: pageSize.toString() }
        })
      );
      
      this.storiesData.set(response);
      
    } catch (err: any) {
      this.error.set(err.error);
      console.error('Error fetching stories:', err);
    } finally {
      this.isLoading.set(false);
    }
  }

  async searchStories(searchRequest: StorySearchRequest) {
    this.isLoading.set(true);
    this.error.set(null);
    
    try {
      const params: Record<string, string> = {};
      if (searchRequest.query) {
        params['query'] = searchRequest.query;
      }

      const response = await firstValueFrom(
        this.http.get<StoriesResponse>(`${this.baseUrl}${API_CONFIG.endpoints.hackernews.search}`, { params })
      );

      this.storiesData.set(response);

    } catch (err: any) {
      this.error.set(err.error);
      console.error('Error searching stories:', err);
    } finally {
      this.isLoading.set(false);
    }
  }
}
