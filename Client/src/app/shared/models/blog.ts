export interface Blog {
  id: number;
  title: string;
  slug: string;
  excerpt: string;
  status: string;
  thumbnailUrl: string;
  createAt: string;
}

export interface BlogDetail {
  id: number;
  title: string;
  slug: string;
  excerpt: string;
  content: string;
  status: string;
  thumbnailUrl: string;
  createAt: string;
}
