export const KNOWN_LANGUAGES = new Set([
  'javascript', 'typescript', 'python', 'java', 'c#', 'c++', 'c', 'go', 'rust',
  'kotlin', 'swift', 'php', 'ruby', 'scala', 'dart', 'r', 'matlab', 'haskell',
  'elixir', 'clojure', 'lua', 'perl', 'shell', 'bash', 'powershell', 'sql',
  'html', 'css', 'sass', 'less',
  'react', 'angular', 'vue', 'svelte', 'nextjs', 'nuxtjs', 'express', 'nestjs',
  'django', 'flask', 'fastapi', 'spring', 'laravel', 'rails', 'asp.net', 'node.js',
  'nodejs', '.net', 'dotnet',
])

export const isKnownLanguage = (lang: string) => KNOWN_LANGUAGES.has(lang.toLowerCase())
