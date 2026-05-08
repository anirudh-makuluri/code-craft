import { CodeCraft } from '@/types/CodeCraft';
import Main from '../components/HomePage';
import { API_BASE_URL } from '@/lib/utils';

async function getCrafts() {
  const res = await fetch(`${API_BASE_URL}/api/crafts/public`, { cache: 'no-store' });
  if (!res.ok) return [];
  return (await res.json()) as CodeCraft[];
}

export default async function Page() {
  const crafts = await getCrafts();
  return <Main crafts={crafts} />;
}
