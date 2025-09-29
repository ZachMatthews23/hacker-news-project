import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { StoryCardComponent } from './components/story-card/story-card.component';
import { StoriesService } from './services/stories.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule, MatButtonModule, MatCardModule, MatIconModule, MatPaginatorModule, MatProgressSpinnerModule, StoryCardComponent],
  template: `
    <main class="app-shell mat-typography">
      <header class="hero">
        <div class="hero__text">
          <h1>Top Hacker News Stories</h1>
          <p class="hero__tagline">Trending discussions curated from the Hacker News community.</p>
        </div>

        <form #useForm="ngForm" class="search-form" (ngSubmit)="searchStories(searchInput.value)">
          <input placeholder="Search stories..." #searchInput (input)="handleInputChanged(searchInput.value)"/>
          <button
            mat-raised-button
            color="accent"
            type="submit"
            [disabled]="loading()"
          >
            {{ loading() ? 'Searching…' : 'Search' }}
          </button>
        </form>
      </header>

      @if (loading()) {
        <div class="status status--loading">
          <mat-progress-spinner strokeWidth="4" diameter="40" mode="indeterminate"></mat-progress-spinner>
          <span>Loading fresh stories…</span>
        </div>
      } @else if (error()) {
        <mat-card appearance="outlined" class="status status--error">
          <mat-card-title>We hit a snag</mat-card-title>
          <mat-card-content>
            <p>{{ error() }}</p>
          </mat-card-content>
        </mat-card>
      } @else if (data()) {
        <section class="stories-grid">
          @for (story of data()?.stories; track story.id) {
            <app-story-card [story]="story" />
          } @empty {
            <p class="status status--empty">No Stories Available.</p>
          }
        </section>
        <mat-paginator 
          [length]="data()?.totalCount" 
          [pageSize]="20" 
          [pageIndex]="pageIndex" 
          [showFirstLastButtons]="true"
          (page)="handlePageEvent($event)"
        ></mat-paginator>
      }
    </main>
  `,
  styleUrl: './app.css'
})
export class App implements OnInit {
  private readonly storiesService = inject(StoriesService);

  readonly data = this.storiesService.storiesData;
  readonly loading = this.storiesService.isLoading;
  readonly error = this.storiesService.error;

  pageIndex = 0;
  pageEvent!: PageEvent;

  ngOnInit() {
    this.fetchAllStories();
  }

  searchStories(query: string) {
    query ? this.storiesService.searchStories({ query }) : null;
  }
  
  fetchAllStories(page?: number) {
    this.storiesService.fetchNewestStories(page);
  }

  handleInputChanged(value: string) {
    if (!value) {
      this.fetchAllStories(1);
      this.pageIndex = 0;
    }
  }

  handlePageEvent(e: PageEvent) {
    this.pageEvent = e;
    this.pageIndex = e.pageIndex;
    this.fetchAllStories(e.pageIndex + 1);
  }
}
