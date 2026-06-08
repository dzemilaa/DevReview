import type { CommentType } from './enums'

export interface Comment {
  id: string
  content: string
  type: CommentType
  startLine: number | null
  endLine: number | null
  suggestedCode: string | null
  isResolved: boolean
  createdAt: string
  userId: string
  authorName: string
  codeFileId: string | null
  codeFilePath: string | null
  parentCommentId: string | null
  replies: Comment[]
}

export interface AddCommentPayload {
  content: string
  commentType: CommentType
  codeFilePath?: string
  startLine?: number
  endLine?: number
  suggestedCode?: string
  parentCommentId?: string
}
