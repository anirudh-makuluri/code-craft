export type CodeCraft = {
  craftId: string;
  name: string;
  createdBy: string;
  js: string;
  css: string;
  html: string;
  isPublic: boolean;
  isFork: boolean;
  likesCount: number;
  viewsCount: number;
  likedBy?: string;
  isLikedByCurrentUser?: boolean;
  parentCraftId?: string | null;
};
