"use client"
import CraftCard from '@/components/CraftCard';
import { CodeCraft } from '@/types/CodeCraft';
import { useEffect, useState } from 'react';
import Header from './Header';
import { API_BASE_URL } from '@/lib/utils';

export default function Main() {
    const [crafts, setCrafts] = useState<CodeCraft[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        let cancelled = false;

        async function loadCrafts() {
            try {
                const res = await fetch(`${API_BASE_URL}/api/crafts/public`);
                if (!res.ok) {
                    if (!cancelled) {
                        setCrafts([]);
                    }
                    return;
                }

                const data = (await res.json()) as CodeCraft[];
                if (!cancelled) {
                    setCrafts(data);
                }
            } finally {
                if (!cancelled) {
                    setLoading(false);
                }
            }
        }

        loadCrafts();

        return () => {
            cancelled = true;
        };
    }, []);

    return (
        <>
            <Header/>
            <main className='flex flex-1 items-center justify-center flex-col gap-8 py-9'>
                {loading && <div className='text-sm text-muted-foreground'>Loading crafts...</div>}
                {!loading && crafts.length === 0 && (
                    <div className='text-sm text-muted-foreground'>No public crafts yet.</div>
                )}
                {
                    crafts.map((craft, index) => (
                        <CraftCard craft={craft} key={index} />
                    ))
                }
            </main>
        </>
    )
}
