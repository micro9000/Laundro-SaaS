'use client'

export function extractInitials(name: string | null | undefined): string {
	if (!name) return '';

	console.log(name);

	const ignoreList = ["ext"]; // titles to ignore

	return name
			.split(',')
			.filter(part => !ignoreList.includes(part.toLowerCase()))
			.map(part => part.trim())
			.map(part => part[0])
			.join('')
			.toUpperCase();
}