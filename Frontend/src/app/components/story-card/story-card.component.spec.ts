import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Story } from '@app/shared/types';
import { StoryCardComponent } from './story-card.component';

describe('StoryCardComponent', () => {
  let fixture: ComponentFixture<StoryCardComponent>;
  let component: StoryCardComponent;

  const baseStory: Story = {
    id: 1,
    title: 'Test Story',
    url: 'https://example.com/story',
    by: 'tester',
    score: 57,
    descendants: 12,
    time: 1710000000,
    type: 'story'
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StoryCardComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(StoryCardComponent);
    component = fixture.componentInstance;
  });

  it('should render story details', () => {
    component.story = { ...baseStory };

    fixture.detectChanges();

    const element: HTMLElement = fixture.nativeElement;
    expect(element.querySelector('.story-link')?.textContent).toContain('Test Story');
    expect(element.textContent).toContain('Score: 57');
    expect(element.textContent).toContain('Comments: 12');
  });

  it('should fallback to Hacker News item link when url missing', () => {
    component.story = { ...baseStory, id: 42, url: '' };

    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    const anchor = element.querySelector('a.story-link') as HTMLAnchorElement | null;
    expect(anchor).not.toBeNull();
    expect(anchor!.getAttribute('href')).toBe('https://news.ycombinator.com/item?id=42');
  });

  it('should apply title attribute to improve accessibility', () => {
    component.story = { ...baseStory, title: 'Readable Title' };

    fixture.detectChanges();

    const element = fixture.nativeElement as HTMLElement;
    const anchor = element.querySelector('a.story-link') as HTMLAnchorElement | null;
    expect(anchor?.getAttribute('title')).toBe('Readable Title');
  });
});
