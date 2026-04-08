import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-section-card',
  standalone: true,
  template: `
    <section class="section-card">
      @if (title || subtitle) {
        <header class="section-card__header">
          <div>
            @if (title) {
              <h2>{{ title }}</h2>
            }
            @if (subtitle) {
              <p>{{ subtitle }}</p>
            }
          </div>
          <ng-content select="[card-action]"></ng-content>
        </header>
      }
      <div class="section-card__body">
        <ng-content></ng-content>
      </div>
    </section>
  `,
  styleUrl: './section-card.component.scss'
})
export class SectionCardComponent {
  @Input() title = '';
  @Input() subtitle = '';
}

