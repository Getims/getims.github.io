// Images from window.__A (data URIs inlined by the build).
declare global { interface Window { __A: Record<string, string> } }

export const images: Record<string, HTMLImageElement> = {};

export function assetUrl(key: string): string {
  return window.__A[key] || '';
}

export async function loadAssets() {
  const jobs = Object.entries(window.__A || {}).filter(([, uri]) => uri.startsWith('data:image')).map(([key, uri]) => {
    const img = new Image();
    img.src = uri;
    images[key] = img;
    return img.decode().catch(() => console.error(`asset decode failed: ${key}`));
  });
  await Promise.all([...jobs, document.fonts ? document.fonts.ready : Promise.resolve()]);
}
