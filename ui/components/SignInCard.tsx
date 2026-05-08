import React from 'react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import zod from 'zod';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { useToast } from '@/components/ui/use-toast';
import { useUser } from '@/app/providers';
import { Checkbox } from '@/components/ui/checkbox';
import { customFetch } from '@/lib/utils';

const formSchema = zod.object({
  username: zod.string().min(2).max(25),
  password: zod.string().min(4),
  remember: zod.boolean().default(false).optional()
});

export default function SignInCard() {
  const { toast } = useToast();
  const { applyAuth } = useUser();

  const form = useForm<zod.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: { password: '', username: '', remember: false }
  });

  async function onSubmit(values: zod.infer<typeof formSchema>) {
    try {
      const data = await customFetch({ pathName: 'auth/login', method: 'POST', body: values });
      applyAuth(data.token, data.user);
    } catch (error: any) {
      toast({ variant: 'destructive', title: 'Sign in failed', description: error.message ?? 'Please try again later' });
    }
  }

  return (
    <Card className={'w-[380px] h-[85vh] flex flex-col items-center justify-center'}>
      <CardHeader className='text-center'>
        <CardTitle>Sign In to your account</CardTitle>
        <CardDescription>Enter your username and password to sign in to your account</CardDescription>
      </CardHeader>
      <CardContent className='grid w-full items-center gap-6'>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-3 w-full'>
            <FormField control={form.control} name='username' render={({ field }) => (<FormItem><FormLabel>Username</FormLabel><FormControl><Input placeholder='SuperCat333' {...field} /></FormControl><FormMessage /></FormItem>)} />
            <FormField control={form.control} name='password' render={({ field }) => (<FormItem><FormLabel>Password</FormLabel><FormControl><Input type='password' placeholder='$Tr0nG!@99' {...field} /></FormControl><FormMessage /></FormItem>)} />
            <FormField control={form.control} name='remember' render={({ field }) => (<FormItem className='flex flex-row items-center gap-2 justify-start'><FormControl><Checkbox id='remember-check' checked={field.value} onCheckedChange={field.onChange} /></FormControl><FormLabel style={{ marginTop: '0px' }}>Remember me</FormLabel></FormItem>)} />
            <Button className='w-full'>Sign In with Email</Button>
          </form>
        </Form>
      </CardContent>
    </Card>
  );
}
