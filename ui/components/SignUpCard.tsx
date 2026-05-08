import React from 'react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import zod from 'zod';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from '@/components/ui/form';
import { useToast } from '@/components/ui/use-toast';
import { customFetch } from '@/lib/utils';

const formSchema = zod.object({
  email: zod.string().email(),
  password: zod.string().min(8),
  confirm_password: zod.string().min(8),
  username: zod.string().min(2).max(25),
  name: zod.string().min(2).max(25)
}).superRefine((val, ctx) => {
  if (val.confirm_password !== val.password) {
    ctx.addIssue({ code: zod.ZodIssueCode.custom, message: 'The passwords did not match' });
  }
});

export default function SignUpCard({ toggleAuthType }: { toggleAuthType: any }) {
  const { toast } = useToast();

  const form = useForm<zod.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: { email: '', password: '', confirm_password: '', username: '', name: '' }
  });

  async function onSubmit(values: zod.infer<typeof formSchema>) {
    try {
      await customFetch({
        pathName: 'auth/register',
        method: 'POST',
        body: { username: values.username, email: values.email, password: values.password, name: values.name }
      });
      toast({ description: 'Account created. You can sign in now.' });
      toggleAuthType('signin');
    } catch (error: any) {
      toast({ variant: 'destructive', title: 'Sign up failed', description: error.message ?? 'Please try again later' });
    }
  }

  return (
    <Card className='w-[380px] h-[85vh]'>
      <CardHeader className='text-center'>
        <CardTitle>Create an account</CardTitle>
        <CardDescription>Fill up the following details to create your account</CardDescription>
      </CardHeader>
      <CardContent className='grid w-full items-center gap-6'>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className='space-y-3 w-full'>
            <FormField control={form.control} name='email' render={({ field }) => (<FormItem><FormLabel>Email</FormLabel><FormControl><Input placeholder='example@mail.com' {...field} /></FormControl><FormMessage /></FormItem>)} />
            <FormField control={form.control} name='password' render={({ field }) => (<FormItem><FormLabel>Password</FormLabel><FormControl><Input type='password' placeholder='$Tr0nG!@99' {...field} /></FormControl><FormMessage /></FormItem>)} />
            <FormField control={form.control} name='confirm_password' render={({ field }) => (<FormItem><FormLabel>Confirm Password</FormLabel><FormControl><Input type='password' placeholder='$Tr0nG!@99' {...field} /></FormControl><FormMessage /></FormItem>)} />
            <FormField control={form.control} name='username' render={({ field }) => (<FormItem><FormLabel>Username</FormLabel><FormControl><Input placeholder='SuperCat333' {...field} /></FormControl><FormMessage /></FormItem>)} />
            <FormField control={form.control} name='name' render={({ field }) => (<FormItem><FormLabel>Name</FormLabel><FormControl><Input placeholder='SuperCat333' {...field} /></FormControl><FormMessage /></FormItem>)} />
            <Button className='w-full'>Sign Up with Email</Button>
          </form>
        </Form>
      </CardContent>
    </Card>
  );
}
