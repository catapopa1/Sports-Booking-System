import { ChangeDetectionStrategy, Component } from '@angular/core';

interface AccentSwatch {
  /** Stable id like "blue-1" or "yellow-3" */
  id: string;
  /** Display name */
  name: string;
  /** Primary accent hex */
  hex: string;
  /** Slightly lighter tint for hover-glow, soft chip backgrounds */
  tint: string;
  /** Slightly darker tone for pressed / deep accent text on light bg */
  deep: string;
  /** Text color that reads on the accent (#FFF or #0A0E14) */
  ink: string;
}

interface AccentGroup {
  title: string;
  description: string;
  swatches: AccentSwatch[];
}

@Component({
  selector: 'app-palettes-preview',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="palettes-page">
      <header class="palettes-page__header">
        <h1 class="palettes-page__title">Accent picker</h1>
        <p class="palettes-page__subtitle">
          Same dark carbon + cool gray base — only the accent (replacing Volt) changes per swatch. Each card shows the
          accent in context: dark hero chip, field-card border on hover, CTA button. Tell me the id (e.g. <b>blue-3</b>)
          and I'll lock it in globally.
        </p>
      </header>

      @for (group of groups; track group.title) {
        <section class="group">
          <header class="group__header">
            <h2 class="group__title">{{ group.title }}</h2>
            <p class="group__desc">{{ group.description }}</p>
          </header>

          <div class="group__grid">
            @for (s of group.swatches; track s.id) {
              <article class="swatch-card"
                       [style.--accent]="s.hex"
                       [style.--accent-tint]="s.tint"
                       [style.--accent-deep]="s.deep"
                       [style.--accent-ink]="s.ink">
                <!-- ID strip + name + hex -->
                <header class="swatch-card__head">
                  <span class="swatch-card__id">{{ s.id }}</span>
                  <h3 class="swatch-card__name">{{ s.name }}</h3>
                  <code class="swatch-card__hex">{{ s.hex }}</code>
                </header>

                <!-- Color blocks: solid + tint + deep -->
                <div class="swatch-card__blocks">
                  <span class="block block--main"></span>
                  <span class="block block--tint"></span>
                  <span class="block block--deep"></span>
                </div>

                <!-- Mini dark hero with accent chip -->
                <div class="mock-hero">
                  <span class="mock-hero__chip">
                    <span class="dot"></span>
                    Bucharest
                  </span>
                  <div class="mock-hero__title">
                    <span class="title-shimmer">Park Gheorgheni</span>
                  </div>
                </div>

                <!-- Mini field card -->
                <div class="mock-row">
                  <div class="mock-row__avatar">
                    <svg viewBox="0 0 24 24" width="24" height="24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round">
                      <circle cx="12" cy="12" r="10"></circle>
                      <path d="M12 2l2 4-2 4-2-4z"></path>
                      <path d="M12 14l2 4-2 4-2-4z"></path>
                      <path d="M2 12l4-2 4 2-4 2z"></path>
                      <path d="M14 12l4-2 4 2-4 2z"></path>
                    </svg>
                  </div>
                  <div class="mock-row__body">
                    <div class="mock-row__title">Teren 1 Fotbal</div>
                    <div class="mock-row__tag">FOOTBALL</div>
                  </div>
                  <button class="mock-row__cta">Book →</button>
                </div>
              </article>
            }
          </div>
        </section>
      }
    </div>
  `,
  styleUrl: './palettes-preview.scss'
})
export class PalettesPreviewComponent {
  readonly groups: AccentGroup[] = [
    {
      title: 'Blue (normal)',
      description: 'Confident, broadly-recognised sports brand blues',
      swatches: [
        { id: 'blue-1', name: 'Royal',    hex: '#2563EB', tint: '#DBEAFE', deep: '#1E40AF', ink: '#FFFFFF' },
        { id: 'blue-2', name: 'Electric', hex: '#0066FF', tint: '#CCE0FF', deep: '#0047B3', ink: '#FFFFFF' },
        { id: 'blue-3', name: 'Sapphire', hex: '#1E40AF', tint: '#C7D5F2', deep: '#172E78', ink: '#FFFFFF' },
        { id: 'blue-4', name: 'Cobalt',   hex: '#3B82F6', tint: '#DDE9FE', deep: '#2563EB', ink: '#FFFFFF' },
        { id: 'blue-5', name: 'Azure',    hex: '#0EA5E9', tint: '#D2EFFB', deep: '#0284C7', ink: '#FFFFFF' },
      ]
    },
    {
      title: 'Blue (light)',
      description: 'Softer blues — feel like courts at dawn',
      swatches: [
        { id: 'lblue-1', name: 'Sky',       hex: '#87CEEB', tint: '#E0F2FA', deep: '#3DA8D0', ink: '#0A0E14' },
        { id: 'lblue-2', name: 'Ice',       hex: '#4FB3FF', tint: '#D6EDFF', deep: '#1D8FE8', ink: '#0A0E14' },
        { id: 'lblue-3', name: 'Powder',    hex: '#B0E0E6', tint: '#E8F6F8', deep: '#7BBEC6', ink: '#0A0E14' },
        { id: 'lblue-4', name: 'Steel',     hex: '#5DA8E8', tint: '#DBEBF9', deep: '#3786C2', ink: '#0A0E14' },
        { id: 'lblue-5', name: 'Cyan',      hex: '#38BDF8', tint: '#D8F1FC', deep: '#0891B2', ink: '#0A0E14' },
      ]
    },
    {
      title: 'Yellow',
      description: 'Warm energy — from saffron to citrus',
      swatches: [
        { id: 'yellow-1', name: 'Saffron',  hex: '#F4C430', tint: '#FCE7A8', deep: '#C99A1C', ink: '#1A1A1A' },
        { id: 'yellow-2', name: 'Amber',    hex: '#FFC107', tint: '#FFE49B', deep: '#C7950B', ink: '#1A1A1A' },
        { id: 'yellow-3', name: 'Mustard',  hex: '#E0B33C', tint: '#F3DD9C', deep: '#B68A28', ink: '#1A1A1A' },
        { id: 'yellow-4', name: 'Citrus',   hex: '#FFD93D', tint: '#FFEDA0', deep: '#C9A615', ink: '#1A1A1A' },
        { id: 'yellow-5', name: 'Sun',      hex: '#FACC15', tint: '#FEEFA1', deep: '#CA8A04', ink: '#1A1A1A' },
      ]
    },
    {
      title: 'Red',
      description: 'Bold, kinetic — Strava / Adidas energy',
      swatches: [
        { id: 'red-1', name: 'Crimson',  hex: '#DC2626', tint: '#FCC7C7', deep: '#991B1B', ink: '#FFFFFF' },
        { id: 'red-2', name: 'Coral',    hex: '#FF6B6B', tint: '#FFD6D6', deep: '#D14848', ink: '#FFFFFF' },
        { id: 'red-3', name: 'Tomato',   hex: '#FF5247', tint: '#FFD0CD', deep: '#CC362D', ink: '#FFFFFF' },
        { id: 'red-4', name: 'Rose',     hex: '#F43F5E', tint: '#FFCBD4', deep: '#BE123C', ink: '#FFFFFF' },
        { id: 'red-5', name: 'Brick',    hex: '#B91C1C', tint: '#F0BFBF', deep: '#7F1313', ink: '#FFFFFF' },
      ]
    },
    {
      title: 'Cream',
      description: 'Warm neutrals — quietly premium',
      swatches: [
        { id: 'cream-1', name: 'Vanilla',   hex: '#F4F0E5', tint: '#FBF9F2', deep: '#C4B98C', ink: '#1A1A1A' },
        { id: 'cream-2', name: 'Beige',     hex: '#E8DCC4', tint: '#F4ECDA', deep: '#B8A581', ink: '#1A1A1A' },
        { id: 'cream-3', name: 'Champagne', hex: '#F7E7CE', tint: '#FCF4E5', deep: '#C9AE7F', ink: '#1A1A1A' },
        { id: 'cream-4', name: 'Ivory',     hex: '#FFFFF0', tint: '#FFFFF8', deep: '#C9C99C', ink: '#1A1A1A' },
        { id: 'cream-5', name: 'Linen',     hex: '#F5F0DC', tint: '#FAF6E9', deep: '#C2B788', ink: '#1A1A1A' },
      ]
    },
    {
      title: 'Green',
      description: 'Pitch-coded — fresh, kinetic, alive',
      swatches: [
        { id: 'green-1', name: 'Emerald', hex: '#10B981', tint: '#C8F0DE', deep: '#047857', ink: '#FFFFFF' },
        { id: 'green-2', name: 'Forest',  hex: '#15803D', tint: '#C7E3CC', deep: '#0E5A2A', ink: '#FFFFFF' },
        { id: 'green-3', name: 'Mint',    hex: '#34D399', tint: '#D2F5E5', deep: '#10B981', ink: '#0A0E14' },
        { id: 'green-4', name: 'Olive',   hex: '#65A30D', tint: '#DFEFC1', deep: '#3F6212', ink: '#FFFFFF' },
        { id: 'green-5', name: 'Teal',    hex: '#14B8A6', tint: '#C8EFEA', deep: '#0F766E', ink: '#FFFFFF' },
      ]
    }
  ];
}
