export const API_CONFIG = {
  baseUrl: 'http://localhost:5163/api',
  endpoints: {
    hackernews: {
      newest: '/hackernews/newest',
      search: '/hackernews/search',
      topStories: '/hackernews/topstories',
    }
  }
} as const;

export const EXTERNAL_URLS = {
  hackerNews: {
    baseItem: 'https://news.ycombinator.com/item?id=',
    baseDomain: 'https://news.ycombinator.com'
  }
} as const;
