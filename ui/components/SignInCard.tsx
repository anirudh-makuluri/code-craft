import React, { MouseEventHandler } from 'react'
import { Button } from '@/components/ui/button';
import {
    Card,
    CardContent,
    CardDescription,
    CardHeader,
    CardTitle,
} from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import zod from "zod";
import { useForm } from "react-hook-form";
import { zodResolver } from '@hookform/resolvers/zod';
import {
    Form,
    FormControl,
    FormDescription,
    FormField,
    FormItem,
    FormLabel,
    FormMessage,
} from "@/components/ui/form"
import { useToast } from "@/components/ui/use-toast";
import Cookies from "universal-cookie"
import { useUser } from '@/app/providers';
import { Checkbox } from "@/components/ui/checkbox"
import { customFetch } from '@/lib/utils';

const formSchema = zod.object({
    username: zod.string().min(2).max(25),
    password: zod.string().min(4),
    remember: zod.boolean().default(false).optional()
})

type Response = {
    success : string,
    error :string,
    token: string,
}

export default function SignInCard() {
    const { toast } = useToast();
    const { login } = useUser();

    const form = useForm<zod.infer<typeof formSchema>>({
        resolver: zodResolver(formSchema),
        defaultValues: {
            password: "",
            username: "",
            remember: false
        }
    })

    async function onSubmit(values: zod.infer<typeof formSchema>) {
        customFetch({ pathName: 'auth/login', method: "POST", body: values })
            .then((data : Response) => {
                if (data.error) {
                    throw new Error(data.error)
                }

                console.log(data);
                login();
            })
            .catch(error => {
                console.error(error);
                toast({
                    variant: "destructive",
                    title: "Uh oh! Something went wrong.",
                    description: error.message ?? "Please try again later",
                }) 
            });
    }

    return (
        <Card className={"w-[380px] h-[85vh] flex flex-col items-center justify-center"}>
            <CardHeader className='text-center'>
                <CardTitle>Sign In to your account</CardTitle>
                <CardDescription>Enter your username and password to sign in to your account</CardDescription>
            </CardHeader>
            <CardContent className='grid w-full items-center gap-6'>
                <Form {...form}>
                    <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-3 w-full">
                        <FormField
                            control={form.control}
                            name="username"
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Username</FormLabel>
                                    <FormControl>
                                        <Input placeholder="SuperCat333" {...field} />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name="password"
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Password</FormLabel>
                                    <FormControl>
                                        <Input type='password' placeholder="$Tr0nG!@99" {...field} />
                                    </FormControl>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />
                        <FormField
                            control={form.control}
                            name="remember"
                            render={({ field }) => (
                                <FormItem className='flex flex-row items-center gap-2 justify-start'>
                                    <FormControl>
                                        <Checkbox id='remember-check' checked={field.value} onCheckedChange={field.onChange} />
                                    </FormControl>
                                    <FormLabel style={{ marginTop: "0px" }}>Remember me</FormLabel>
                                </FormItem>
                            )}
                        />
                        <div className='flex flex-row justify-between'>
                            <span className='whitespace-nowrap text-xs underline opacity-0'>Forgot password?</span>
                        </div>
                        <Button className='w-full'>Sign In with Email</Button>
                    </form>
                </Form>
                {/* <Separator title='or continue with' />
                <Button className='gap-2' variant={"outline"}><Icons.google className='w-3' /> Sign In with Google</Button> */}
            </CardContent>
        </Card>
    )
}
