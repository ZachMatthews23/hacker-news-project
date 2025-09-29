export interface StorySearchRequest {
  query: string;
}

export interface Story {
  id: number;
  title: string;
  url: string;
  score: number;
  by: string;
  time: number;
  descendants: number;
  type: string;
};

export interface StoriesResponse {
  stories: Story[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
};