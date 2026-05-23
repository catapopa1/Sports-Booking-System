/**
 * Procedural canvas textures for sports balls.
 * Each generator returns a 2:1 (equirectangular) canvas suitable for use with
 * THREE.CanvasTexture mapped onto a SphereGeometry with default UVs.
 *
 * Coordinate system: x ∈ [0, W) → longitude, y ∈ [0, H) → latitude.
 */

const W = 1024;
const H = 512;

function makeCanvas(): HTMLCanvasElement {
  const c = document.createElement('canvas');
  c.width = W;
  c.height = H;
  return c;
}

/** Convert (longitude in deg, latitude in deg) to canvas pixel (x, y). */
function lonLatToPx(lon: number, lat: number): [number, number] {
  // lon ∈ [-180, 180] → x; lat ∈ [-90, 90] → y (lat=+90 is top of texture)
  const x = ((lon + 180) / 360) * W;
  const y = ((90 - lat) / 180) * H;
  return [x, y];
}

/** Draw a filled circle at (lon, lat) with a given radius in degrees. */
function drawSpot(
  ctx: CanvasRenderingContext2D,
  lon: number,
  lat: number,
  radiusDeg: number,
  fill: string
): void {
  const [cx, cy] = lonLatToPx(lon, lat);
  const radiusPx = (radiusDeg / 360) * W;

  // Draw the circle plus its wrap-around copies for longitude continuity at
  // the texture seam (x=0 / x=W). Also draw a stretched copy near the poles.
  for (const dx of [-W, 0, W]) {
    ctx.beginPath();
    ctx.fillStyle = fill;
    ctx.arc(cx + dx, cy, radiusPx, 0, Math.PI * 2);
    ctx.fill();
  }
}

/* ── Football: white sphere with 12 black pentagonal patches in
 * icosahedral arrangement (approximated as filled circles).  ────────── */
export function footballTexture(): HTMLCanvasElement {
  const c = makeCanvas();
  const ctx = c.getContext('2d')!;

  // Base white with subtle off-white panels
  ctx.fillStyle = '#F5F5F0';
  ctx.fillRect(0, 0, W, H);

  // Subtle stitching / panel hint via a faint cell pattern
  ctx.fillStyle = 'rgba(0, 0, 0, 0.04)';
  for (let lat = -75; lat <= 75; lat += 15) {
    for (let lon = -180; lon < 180; lon += 18) {
      const [x, y] = lonLatToPx(lon, lat);
      ctx.fillRect(x - 0.5, y - 0.5, 1, 1);
    }
  }

  // 12 black pentagons at icosahedron vertex positions
  // Top + bottom poles
  drawSpot(ctx, 0, 90, 8, '#0A0A0A');
  drawSpot(ctx, 0, -90, 8, '#0A0A0A');
  // 5 upper, lat ≈ +26.57°, lon every 72° offset 0
  for (let i = 0; i < 5; i++) {
    drawSpot(ctx, -180 + 36 + i * 72, 26.57, 10, '#0A0A0A');
  }
  // 5 lower, lat ≈ -26.57°, lon every 72° offset 36°
  for (let i = 0; i < 5; i++) {
    drawSpot(ctx, -180 + 72 + i * 72, -26.57, 10, '#0A0A0A');
  }

  return c;
}

/* ── Tennis ball: bright yellow-green with two white seam curves ─── */
export function tennisTexture(): HTMLCanvasElement {
  const c = makeCanvas();
  const ctx = c.getContext('2d')!;

  // Base yellow-green
  ctx.fillStyle = '#D4E83A';
  ctx.fillRect(0, 0, W, H);

  // Soft texture (subtle horizontal banding from fuzz)
  for (let y = 0; y < H; y++) {
    const n = (Math.sin(y * 0.3) + Math.cos(y * 0.17)) * 0.5;
    ctx.fillStyle = `rgba(190, 220, 60, ${0.04 + Math.abs(n) * 0.06})`;
    ctx.fillRect(0, y, W, 1);
  }

  // Two seam curves: each is a great circle on the sphere, appearing as
  // a sinusoidal curve in equirectangular projection.
  ctx.lineWidth = 14;
  ctx.strokeStyle = '#FFFFFF';
  ctx.lineCap = 'round';

  const drawSeam = (phaseDeg: number) => {
    for (const dx of [-W, 0, W]) {
      ctx.beginPath();
      for (let x = 0; x <= W; x += 2) {
        const lon = (x / W) * 360 - 180;
        // Great-circle latitude for a curve tilted 45° from the equator
        // The seam pattern of a tennis ball can be approximated by this formula.
        const lat = 32 * Math.sin((lon + phaseDeg) * Math.PI / 180);
        const [px, py] = lonLatToPx(lon, lat);
        if (x === 0) ctx.moveTo(px + dx, py);
        else         ctx.lineTo(px + dx, py);
      }
      ctx.stroke();
    }
  };
  drawSeam(0);
  drawSeam(180);

  return c;
}

/* ── Basketball: orange with 4 black seam lines (equator + 3 verticals) */
export function basketballTexture(): HTMLCanvasElement {
  const c = makeCanvas();
  const ctx = c.getContext('2d')!;

  // Base orange
  ctx.fillStyle = '#D96B28';
  ctx.fillRect(0, 0, W, H);

  // Subtle darker mottling for a leather feel
  for (let i = 0; i < 800; i++) {
    const x = Math.random() * W;
    const y = Math.random() * H;
    ctx.fillStyle = `rgba(110, 50, 18, ${0.04 + Math.random() * 0.05})`;
    ctx.beginPath();
    ctx.arc(x, y, 1 + Math.random() * 1.5, 0, Math.PI * 2);
    ctx.fill();
  }

  ctx.lineWidth = 12;
  ctx.strokeStyle = '#1A0F08';
  ctx.lineCap = 'round';

  // 1. Equator (horizontal at lat=0)
  ctx.beginPath();
  const [eqX1, eqY] = lonLatToPx(-180, 0);
  ctx.moveTo(eqX1, eqY);
  ctx.lineTo(W, eqY);
  ctx.stroke();

  // 2. Vertical seam at lon=0 (a meridian — vertical line at x=W/2)
  const [mY1] = lonLatToPx(0, 90);
  const [vX, _] = lonLatToPx(0, 0);
  void _;
  ctx.beginPath();
  ctx.moveTo(vX, 0);
  ctx.lineTo(vX, H);
  ctx.stroke();

  // 3 & 4. Two diagonal "horseshoe" seams — each is a great-circle that
  // crosses the equator twice; appears as a sin-wave on UV.
  const drawHorseshoe = (phaseDeg: number) => {
    for (const dx of [-W, 0, W]) {
      ctx.beginPath();
      for (let x = 0; x <= W; x += 2) {
        const lon = (x / W) * 360 - 180;
        const lat = 55 * Math.sin((lon + phaseDeg) * Math.PI / 180);
        const [px, py] = lonLatToPx(lon, lat);
        if (x === 0) ctx.moveTo(px + dx, py);
        else         ctx.lineTo(px + dx, py);
      }
      ctx.stroke();
    }
  };
  drawHorseshoe(0);
  drawHorseshoe(180);

  return c;
}

export function textureForSport(sport: string): HTMLCanvasElement {
  const s = (sport || '').toLowerCase();
  if (s.startsWith('tennis'))     return tennisTexture();
  if (s.startsWith('basket'))     return basketballTexture();
  return footballTexture();
}
