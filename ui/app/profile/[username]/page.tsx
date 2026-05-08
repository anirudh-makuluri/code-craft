import Main from './main';
import { notFound } from 'next/navigation';
import { API_BASE_URL } from '@/lib/utils';

async function getUserData(username: string) {
  const [userResp, craftResp] = await Promise.all([
    fetch(`${API_BASE_URL}/auth/user?username=${username}`, { cache: 'no-store' }),
    fetch(`${API_BASE_URL}/api/user?username=${username}`, { cache: 'no-store' })
  ]);

  if (!userResp.ok) throw new Error('User not found');
  const userData = await userResp.json();
  const craftData = craftResp.ok ? await craftResp.json() : [];

  return [userData, craftData];
}

export default async function Page({ params }: { params: { username: string } }) {
  try {
    const [userData, craftData] = await getUserData(params.username);
    return <Main userData={userData} craftData={craftData} />;
  } catch {
    notFound();
  }
}
