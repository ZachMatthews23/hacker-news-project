import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { API_CONFIG } from '@app/shared/config';
import { StoriesResponse } from '@app/shared/types';
import { StoriesService } from './stories.service';

describe('StoriesService', () => {
  let service: StoriesService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule]
    });

    service = TestBed.inject(StoriesService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should fetch newest stories and update state', async () => {
    const response: StoriesResponse = {
      stories: [
        {
          id: 1,
          title: 'Fresh Angular Story',
          url: 'https://angular.dev',
          by: 'angular',
          score: 50,
          descendants: 10,
          time: 1710000000,
          type: 'story'
        }
      ],
      totalCount: 1,
      page: 2,
      pageSize: 5,
      totalPages: 1
    };

    const fetchPromise = service.fetchNewestStories(2, 5);

    expect(service.isLoading()).toBeTrue();

    const request = httpMock.expectOne(req => req.url === `${API_CONFIG.baseUrl}${API_CONFIG.endpoints.hackernews.newest}`);
    expect(request.request.method).toBe('GET');
    expect(request.request.params.get('page')).toBe('2');
    expect(request.request.params.get('pageSize')).toBe('5');

    request.flush(response);
    await fetchPromise;

    expect(service.isLoading()).toBeFalse();
    expect(service.error()).toBeNull();
    expect(service.storiesData()).toEqual(response);
  });

  it('should capture errors when fetching newest stories fails', async () => {
    const fetchPromise = service.fetchNewestStories();

    const request = httpMock.expectOne(req => req.url === `${API_CONFIG.baseUrl}${API_CONFIG.endpoints.hackernews.newest}`);
    request.flush('Server error', { status: 500, statusText: 'Server Error' });

    await fetchPromise;

    expect(service.isLoading()).toBeFalse();
    expect(service.storiesData()).toBeNull();
    expect(service.error()).toBe('Server error');
  });

  it('should search stories with a query parameter', async () => {
    const response: StoriesResponse = {
      stories: [],
      totalCount: 0,
      page: 1,
      pageSize: 20,
      totalPages: 0
    };

    const searchPromise = service.searchStories({ query: 'angular' });

    const request = httpMock.expectOne(req => req.url === `${API_CONFIG.baseUrl}${API_CONFIG.endpoints.hackernews.search}`);
    expect(request.request.params.get('query')).toBe('angular');

    request.flush(response);
    await searchPromise;

    expect(service.storiesData()).toEqual(response);
    expect(service.error()).toBeNull();
  });

  it('should search stories without query parameter when empty', async () => {
    const response: StoriesResponse = {
      stories: [],
      totalCount: 0,
      page: 1,
      pageSize: 20,
      totalPages: 0
    };

    const searchPromise = service.searchStories({ query: '' });

    const request = httpMock.expectOne(req => req.url === `${API_CONFIG.baseUrl}${API_CONFIG.endpoints.hackernews.search}`);
    expect(request.request.params.keys().length).toBe(0);

    request.flush(response);
    await searchPromise;

    expect(service.storiesData()).toEqual(response);
  });
});
