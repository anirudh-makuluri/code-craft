"use client"
import React, { useState, useEffect } from 'react';
import SignUpCard from '@/components/SignUpCard';
import SignInCard from '@/components/SignInCard';
import { useRouter, useSearchParams } from 'next/navigation';
import { motion } from 'framer-motion';
import { useUser } from '../providers';
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"

type authType = "signin" | "signup";

export default function Page() {
    const { user } = useUser();
    const [authType, setAuthType] = useState<authType>("signin");
    const router = useRouter();
    const searchParams = useSearchParams();

    useEffect(() => {
        if (user) {
            if (searchParams.has('redirect')) {
                const redirect = searchParams.get('redirect');
                router.push(`/craft/${redirect}`);
            } else {
                router.push(`/profile/${user.username}`);
            }
        }
    }, [user, router, searchParams]);

    function toggleAuthType(newAuthType : authType) {
        setAuthType(newAuthType);
    }

    return (
        <main className='flex flex-1 items-center justify-center'>
            <motion.div
                initial={{ scale: 0 }}
                animate={{ scale: 1 }}
                transition={{
                    type: "spring",
                    stiffness: 100
                }}
            >
                <Tabs defaultValue="signin" value={authType} className="w-full">
                    <TabsList className='w-full'>
                        <TabsTrigger className='w-1/2 flex items-center justify-center' onClick={() => toggleAuthType("signin")} value="signin">Sign In</TabsTrigger>
                        <TabsTrigger className='w-1/2 flex items-center justify-center' onClick={() => toggleAuthType("signup")} value="signup">Sign Up</TabsTrigger>
                    </TabsList>
                    <TabsContent value="signin"><SignInCard /></TabsContent>
                    <TabsContent value="signup"><SignUpCard toggleAuthType={toggleAuthType}/></TabsContent>
                </Tabs>
            </motion.div>
        </main>
    )
}
