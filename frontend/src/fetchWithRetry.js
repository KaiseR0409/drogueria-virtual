
export async function fetchWithRetry(url, options = {}, maxRetries = 5, totalTimeout = 15000) {
  const delayBetweenRetries = totalTimeout / maxRetries;

  for (let attempt = 1; attempt <= maxRetries; attempt++) {
    try {
      const res = await fetch(url, options);

      if (res.ok) {
      return res;
      }

      if ([500, 502, 503, 504].includes(res.status)) {
        throw new Error(`Servidor respondió con ${res.status}`);
      }

      return res;
    } catch (err) {
      console.warn(`Intento ${attempt} fallido:`, err.message);

      if (attempt === maxRetries) {
        throw new Error("Servidor no disponible tras múltiples intentos");
      }

      await new Promise(r => setTimeout(r, delayBetweenRetries));
    }
  }
}
