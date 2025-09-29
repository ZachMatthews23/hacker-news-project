import { ComponentFixture, TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { By } from '@angular/platform-browser';
import { StoriesResponse } from '@app/shared/types';
import { PageEvent } from '@angular/material/paginator';
import { App } from './app';
import { StoriesService } from './services/stories.service';

class StoriesServiceStub {
  storiesData = signal<StoriesResponse | null>(null);
  isLoading = signal<boolean>(false);
  error = signal<string | null>(null);
  searchStories = jasmine.createSpy('searchStories');
  fetchNewestStories = jasmine.createSpy('fetchNewestStories');
}

describe('App', () => {
  let fixture: ComponentFixture<App>;
  let component: App;
  let storiesService: StoriesServiceStub;

  beforeEach(async () => {
    storiesService = new StoriesServiceStub();

    await TestBed.configureTestingModule({
      imports: [App],
      providers: [{ provide: StoriesService, useValue: storiesService }]
    }).compileComponents();
  });

  function createComponent() {
    fixture = TestBed.createComponent(App);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }

  it('should create and fetch newest stories on init', () => {
    createComponent();

    expect(component).toBeTruthy();
    expect(storiesService.fetchNewestStories).toHaveBeenCalledWith(undefined);
  });

  it('should display a loading spinner while fetching data', () => {
    storiesService.isLoading.set(true);

    createComponent();

    const spinner = fixture.nativeElement.querySelector('mat-progress-spinner');
    expect(spinner).not.toBeNull();
  });

  it('should render story cards and paginator when data is available', () => {
    createComponent();

    storiesService.storiesData.set({
      stories: [
        {
          id: 1,
          title: 'Angular Testing Tools',
          url: 'https://angular.dev',
          by: 'tester',
          score: 100,
          descendants: 42,
          time: 1710000000,
          type: 'story'
        }
      ],
      totalCount: 25,
      page: 1,
      pageSize: 20,
      totalPages: 2
    });

    fixture.detectChanges();

    const cards = fixture.debugElement.queryAll(By.css('app-story-card'));
    const paginator = fixture.debugElement.query(By.css('mat-paginator'));
    expect(cards.length).toBe(1);
    expect(paginator).not.toBeNull();
  });

  it('should display an error message when the service reports an error', () => {
    storiesService.error.set('Something went wrong');

    createComponent();

    const errorText = fixture.nativeElement.textContent;
    expect(errorText).toContain('Something went wrong');
  });

  it('should delegate searches only when a query is present', () => {
    createComponent();
    storiesService.searchStories.calls.reset();

    component.searchStories('Angular');
    component.searchStories('');

    expect(storiesService.searchStories).toHaveBeenCalledTimes(1);
    expect(storiesService.searchStories).toHaveBeenCalledWith({ query: 'Angular' });
  });

  it('should refresh newest stories on paginator interaction', () => {
    createComponent();
    storiesService.fetchNewestStories.calls.reset();

    const event = { pageIndex: 2, pageSize: 20, length: 100 } as PageEvent;
    component.handlePageEvent(event);

    expect(component.pageIndex).toBe(2);
    expect(storiesService.fetchNewestStories).toHaveBeenCalledWith(3);
  });

  it('should reload newest stories when the search input is cleared', () => {
    createComponent();
    storiesService.fetchNewestStories.calls.reset();
    component.pageIndex = 4;

    component.handleInputChanged('');

    expect(component.pageIndex).toBe(0);
    expect(storiesService.fetchNewestStories).toHaveBeenCalledWith(1);
  });
});
