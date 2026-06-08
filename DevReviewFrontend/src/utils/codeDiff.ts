export interface DiffLine {
  type: 'remove' | 'add' | 'context'
  lineNumber?: number
  content: string
}

/** Build a simple line-based diff for display (1-based line numbers). */
export function buildLineDiff(
  fileContent: string,
  startLine: number,
  endLine: number,
  suggestedCode: string,
): DiffLine[] {
  const fileLines = fileContent.split('\n')
  const suggestedLines = suggestedCode.replace(/\r\n/g, '\n').split('\n')
  const result: DiffLine[] = []

  const contextBefore = Math.max(1, startLine - 1)
  if (contextBefore < startLine) {
    result.push({
      type: 'context',
      lineNumber: contextBefore,
      content: fileLines[contextBefore - 1] ?? '',
    })
  }

  for (let i = startLine; i <= endLine; i++) {
    result.push({
      type: 'remove',
      lineNumber: i,
      content: fileLines[i - 1] ?? '',
    })
  }

  suggestedLines.forEach((line, idx) => {
    result.push({
      type: 'add',
      content: line,
      lineNumber: startLine + idx,
    })
  })

  const contextAfter = endLine + 1
  if (contextAfter <= fileLines.length) {
    result.push({
      type: 'context',
      lineNumber: contextAfter,
      content: fileLines[contextAfter - 1] ?? '',
    })
  }

  return result
}

/** Replace a 1-based inclusive line range with suggested code. */
export function applyLineReplacement(
  content: string,
  startLine: number,
  endLine: number,
  suggestedCode: string,
): string {
  const lines = content.split('\n')
  const replacement = suggestedCode.replace(/\r\n/g, '\n').split('\n')
  const startIdx = startLine - 1
  const removeCount = endLine - startLine + 1
  lines.splice(startIdx, removeCount, ...replacement)
  return lines.join('\n')
}

export function commentTypeColor(type: number): string {
  switch (type) {
    case 1:
      return 'border-l-blue-500 bg-blue-50'
    case 2:
      return 'border-l-amber-500 bg-amber-50'
    case 3:
      return 'border-l-emerald-500 bg-emerald-50'
    case 4:
      return 'border-l-red-500 bg-red-50'
    default:
      return 'border-l-slate-400 bg-slate-50'
  }
}

export function lineHighlightColor(type: number): string {
  switch (type) {
    case 1:
      return 'bg-blue-500/15'
    case 2:
      return 'bg-amber-500/15'
    case 3:
      return 'bg-emerald-500/15'
    case 4:
      return 'bg-red-500/15'
    default:
      return 'bg-brand-500/10'
  }
}
