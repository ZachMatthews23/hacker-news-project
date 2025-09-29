import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { Story } from '../../shared/types/api.types';
import { EXTERNAL_URLS } from '../../shared/config';

@Component({
  selector: 'app-story-card',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule],
  template: `
    <mat-card class="story-card" appearance="outlined">
      <mat-card-header>
        <mat-card-title [title]="story.title">
          <a
            class="story-link"
            [href]="story.url || baseItemUrl + story.id"
            target="_blank"
            rel="noopener"
            [title]="story.title"
          >
            {{ story.title }}
          </a>
        </mat-card-title>
        <mat-card-subtitle>By {{ story.by }}</mat-card-subtitle>
      </mat-card-header>

      <mat-card-content>
        <div class="story-meta">
          <span class="story-meta__item">Score: {{ story.score }}</span>
          <span class="story-meta__item">Comments: {{ story.descendants }}</span>
        </div>
      </mat-card-content>

      <mat-card-actions align="end">
        <a
          mat-stroked-button
          color="primary"
          [href]="story.url || baseItemUrl + story.id"
          target="_blank"
          rel="noopener"
        >
          Read full story
        </a>
      </mat-card-actions>
    </mat-card>
  `,
  styleUrl: './story-card.component.css'
})
export class StoryCardComponent {
  @Input({ required: true }) story!: Story;

  protected readonly baseItemUrl = EXTERNAL_URLS.hackerNews.baseItem;
}
